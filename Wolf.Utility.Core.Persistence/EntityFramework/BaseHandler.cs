using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;
using Wolf.Utility.Core.Persistence.Exceptions;
using TypeExtensions = Wolf.Utility.Core.Extensions.Methods.TypeExtensions;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    public abstract class BaseHandler<TContext, TEntity, TSearchable> : IHandler<TEntity, TSearchable>
        where TContext : BaseContext where TEntity : class, IEntity where TSearchable : class, ISearchableEntity, new()
    {
        protected TContext Context { get; }
        protected object ContextLock { get; } = new { };
        private bool SavingChanges;

        protected BaseHandler(TContext context)
        {
            Context = context;
        }


        #region Help Methods

        private async Task Save()
        {
            while (SavingChanges) await Task.Delay(new TimeSpan(0, 0, 1));

            try
            {
                SavingChanges = true;
                await Context.SaveChangesAsync();
            }
            finally
            {
                SavingChanges = false;
            }
        }

        private EntityState CheckEntryState(EntityState state, EntityEntry entry)
        {
            if (entry != null)
                state = entry.State;
            return state;
        }

        private bool VerifyEntryState(EntityState actualState, EntityState desiredState)
        {
            return actualState == desiredState;
        }

        /// <summary>
        /// Retrieves exactly one result, matching the query input. If there are more or less, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query to run, and retrieve Entities via</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is less or more than 1 result found matching the query</exception>
        private TEntity FindOneResult(IQueryable<TEntity> query)
        {
            lock (ContextLock)
            {
                var result = query.ToList();
                return result.Count switch
                {
                    1 => result.First(),
                    > 1 => throw IncorrectEntityCountException<TEntity>.Constructor(1, result.Count, true, result),
                    _ => throw IncorrectEntityCountException<TEntity>.Constructor(1, result.Count,
                        elements: new List<TEntity>())
                };
            }
        }

        /// <summary>
        /// Retrieves one or more results, matching the query input. If there are 0 results, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query to run, and retrieve Entities via</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is 0 results found matching the query</exception>
        private ICollection<TEntity> FindMultipleResults(IQueryable<TEntity> query)
        {
            lock (ContextLock)
            {
                var result = query.ToList();
                if (result.Any())
                    return result;
                throw IncorrectEntityCountException<TEntity>.Constructor(1, result.Count,
                    elements: new List<TEntity>());
            }
        }

        #endregion

        #region Find

        protected abstract IQueryable<TEntity> GetQueryable();

        protected virtual IQueryable<TEntity> BuildFindQuery(TSearchable search)
        {
            var query = GetQueryable();

            if (search.Id != default)
                query = query.Where(x => x.Id == search.Id);

            return query;
        }


        /// <inheritdoc />
        public async Task<TEntity> Find(TSearchable entity)
        {
            var query = BuildFindQuery(entity);
            return FindOneResult(query);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TEntity>> FindMultiple(TSearchable entity)
        {
            var query = BuildFindQuery(entity);
            return FindMultipleResults(query);
        }

        #endregion

        #region Update

        /// <inheritdoc />
        public async Task<TEntity> UpdateAndRetrieve(TEntity entity)
        {
            if (entity.Id == 0)
                throw new ArgumentException($"I need an Id to figure out what to update",
                    new ArgumentException("Id of predicate can not be 0"));

            bool result = await UpdateOperation(entity);
            if (result == false)
                throw new TaskFailedException(
                    TypeExtensions
                        .GetMethodInfo<BaseHandler<TContext, TEntity, TSearchable>>(nameof(UpdateAndRetrieve)),
                    $"Task was suppose to update {nameof(entity)} of type {typeof(TEntity).FullName} in database, but failed to set state to modified");

            await Save();

            TEntity updated = await Find(entity as TSearchable);
            return updated;
        }

        /// <inheritdoc />
        public async Task<ICollection<TEntity>> UpdateMultipleAndRetrieve(ICollection<TEntity> entities)
        {
            if (entities.Any(x => x.Id == 0))
                throw new ArgumentException($"I need an Id to figure out what to update",
                    new ArgumentException("Id of predicate can not be 0"));

            bool[] results = entities.Select(async x => await UpdateOperation(x)).Select(x => x.Result).ToArray();
            if (results.Any(x => x == false))
                throw new TaskFailedException(
                    TypeExtensions
                        .GetMethodInfo<BaseHandler<TContext, TEntity, TSearchable>>(nameof(UpdateAndRetrieve)),
                    $"Task was suppose to update {nameof(entities)} of type {typeof(TEntity).FullName} in database, but failed to set state at least one to modified");

            await Save();

            var updated = entities.Select(async x => await Find(x as TSearchable)).Select(x => x.Result).ToArray();
            return updated;
        }

        protected virtual async Task<bool> UpdateOperation(TEntity entity)
        {
            EntityEntry entry;
            var state = EntityState.Unchanged;

            entry = Context.Update(entity);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Modified);
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public async Task<bool> Delete(TEntity entity)
        {
            if (entity.Id == default)
                throw new ArgumentException($"I need an Id to figure out what to remove",
                    new ArgumentException("Id of predicate can't be 0"));

            bool result = await DeleteOperation(entity);

            await Save();

            return result;
        }

        /// <inheritdoc />
        public async Task<bool> Delete(int id)
        {
            if (id == default)
                throw new ArgumentException($"I need an Id to figure out what to remove",
                    new ArgumentException("Id of predicate can't be 0"));

            var search = new TSearchable() { Id = id };
            TEntity entity = await Find(search);
            bool result = await DeleteOperation(entity);

            await Save();

            return result;
        }

        protected virtual async Task<bool> DeleteOperation(TEntity entity)
        {
            EntityEntry entry;
            var state = EntityState.Unchanged;

            entry = Context.Remove(entity);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Deleted);
        }

        #endregion

        #region Add
        /// <inheritdoc />
        public async Task<TEntity> AddAndRetrieve(TEntity entity, bool tryRetrieveFirst = true)
        {
            if (entity.Id != 0)
                throw new ArgumentException($"I need Id to be 0 to set it properly myself",
                    new ArgumentException($"Id of predicate has to be 0"));

            if (tryRetrieveFirst)
                try
                {
                    TEntity retrieve = await Find(entity as TSearchable);
                    return retrieve;
                }
                catch (IncorrectCountException<TEntity> ice)
                {
                    if (ice.ToMany)
                        throw new Exception(
                            $"While attempting to retrieve before adding, found more than one result matching {nameof(entity)}",
                            ice);
                }

            bool result = await AddOperation(entity);
            if (result == false)
                throw new TaskFailedException(
                    TypeExtensions.GetMethodInfo<BaseHandler<TContext, TEntity, TSearchable>>(nameof(AddAndRetrieve)),
                    $"Task was suppose to add {nameof(entity)} of type {typeof(TEntity).FullName} to database, but failed to set state to added");

            await Save();

            TEntity added = await Find(entity as TSearchable);
            return added;
        }

        /// <inheritdoc />
        public async Task<ICollection<TEntity>> AddMultipleAndRetrieve(ICollection<TEntity> entities,
            bool tryRetrieveFirst = true)
        {
            if (entities.Any(x => x.Id != 0))
                throw new ArgumentException($"I need Id to be 0 to set it properly myself",
                    new ArgumentException($"Id of predicate has to be 0"));

            if (tryRetrieveFirst)
                try
                {
                    var retrieve = entities.Select(async x => await Find(x as TSearchable)).Select(x => x.Result)
                        .ToList();
                    return retrieve;
                }
                catch (IncorrectCountException<TEntity> ice)
                {
                    if (ice.ToMany)
                        throw new Exception(
                            $"While attempting to retrieve before adding, found more than one result matching an entity in {nameof(entities)}",
                            ice);
                }

            var result = entities.Select(async x => await AddOperation(x)).Select(x => x.Result).ToList();
            if (result.Any(x => x == false))
                throw new TaskFailedException(
                    TypeExtensions.GetMethodInfo<BaseHandler<TContext, TEntity, TSearchable>>(nameof(AddAndRetrieve)),
                    $"Task was suppose to add {nameof(entities)} of type {typeof(TEntity).FullName} to database, but failed to set state to added of at least one entity");

            await Save();

            var added = entities.Select(async x => await Find(x as TSearchable)).Select(x => x.Result).ToList();
            return added;
        }

        protected virtual async Task<bool> AddOperation<T>(T entity) where T : class, IEntity
        {
            EntityEntry entry;
            var state = EntityState.Unchanged;

            entry = Context.Add(entity);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Added);
        } 
        #endregion
    }
}