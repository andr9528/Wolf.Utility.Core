using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    public abstract class BaseHandler<TContext> : IHandler where TContext : DbContext
    {
        protected TContext Context { get; }

        protected BaseHandler(TContext context)
        {
            Context = context;
        }

        #region Find

        public async Task<T> Find<T>(T predicate) where T : class, IEntity
        {
            try
            {
                var query = await AbstractFind(predicate);

                var result = FindOneResult(query);

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        public async Task<IEnumerable<T>> FindMultiple<T>(T predicate) where T : class, IEntity
        {
            try
            {
                var query = await AbstractFind(predicate);

                var result = FindMultipleResults(query);

                return result;
            }
            catch (Exception e)
            {
                throw;
            }
        }

        protected abstract Task<IQueryable<T>> AbstractFind<T>(T predicate) where T : class, IEntity;

        #endregion

        #region Update

        public async Task<bool> Update<T>(T element, bool autoSave = true) where T : class, IEntity
        {
            if (element.Id == 0)
                throw new Exception($"I need an Id to figure out what to update", new ArgumentException("Id of predicate can not be 0"));

            var result = await VirtualUpdate(element);

            return false;
        }

        protected virtual async Task<bool> VirtualUpdate<T>(T element) where T : class, IEntity
        {
            EntityEntry entry = null;
            EntityState state = EntityState.Unchanged;

            entry = Context.Update(element);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Modified);
        }

        #endregion

        #region Delete

        public async Task<bool> Delete<T>(T element, bool autoSave = true) where T : class, IEntity
        {
            if (element.Id == 0)
                throw new Exception($"I need an Id to figure out what to remove", new ArgumentException("Id of predicate can't be 0"));

            var result = await VirtualDelete(element);

            if (autoSave)
                await Save();

            return result;
        }

        protected virtual async Task<bool> VirtualDelete<T>(T element) where T : class, IEntity
        {
            EntityEntry entry = null;
            EntityState state = EntityState.Unchanged;

            entry = Context.Remove(element);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Deleted);
        }


        #endregion

        #region Add

        public async Task<bool> Add<T>(T element, bool autoSave = true) where T : class, IEntity
        {
            if (element.Id != 0)
                throw new Exception($"I need Id to be 0 to set it properly myself", new ArgumentException($"Id of predicate has to be 0"));

            var result = await VirtualAdd(element);

            if (autoSave)
                await Save();

            return result;
        }

        protected virtual async Task<bool> VirtualAdd<T>(T element) where T : class, IEntity
        {
            EntityEntry entry = null;
            EntityState state = EntityState.Unchanged;

            entry = Context.Add(element);

            state = CheckEntryState(state, entry);
            return VerifyEntryState(state, EntityState.Added);
        }

        #endregion

        #region AddMultiple

        public async Task<string> AddMultiple<T>(ICollection<T> elements) where T : class, IEntity
        {
            var added = await VirtualAddMultiple(elements);
            var result = GetAmountAdded(added);

            await Save();

            return result;
        }

        protected virtual async Task<ICollection<bool>> VirtualAddMultiple<T>(ICollection<T> elements)
            where T : class, IEntity
        {
            var results = new List<bool>();

            foreach (var element in elements)
            {
                results.Add(await Add(element, false));
            }

            return results;
        }

        #endregion

        #region Help Methods
        public async Task Save()
        {
            await Context.SaveChangesAsync();
        }

        internal EntityState CheckEntryState(EntityState state, EntityEntry entry)
        {
            if (entry != null)
                state = entry.State;
            return state;
        }

        internal bool VerifyEntryState(EntityState actualState, EntityState desiredState)
        {
            return actualState == desiredState ? true : false;
        }

        internal string GetAmountAdded(ICollection<bool> results)
        {
            return $"Added {results.Count(b => b)} out of {results.Count}.";
        }

        private T FindOneResult<T>(IQueryable<T> query) where T : class, IEntity
        {
            var result = query.ToList();
            if (result.Count() == 1)
                return result.First();
            if (result.Count() > 1)
                throw IncorrectResultCountException<T>.Constructor(1, result.Count, true, result);
            throw IncorrectResultCountException<T>.Constructor(1, result.Count, elements: new List<T>());
        }

        private ICollection<T> FindMultipleResults<T>(IQueryable<T> query) where T : class, IEntity
        {
            var result = query.ToList();
            if (result.Any())
                //return new List<T>(result);
                return result;
            throw IncorrectResultCountException<T>.Constructor(1, result.Count, elements: new List<T>());
        }

        #endregion
    }
}

