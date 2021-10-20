using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Extensions.Methods;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework
{
    public abstract class BaseHandler<TContext> : IHandler where TContext : BaseContext
    {
        protected TContext Context { get; }
        protected object ContextLock { get; } = new { };

        protected BaseHandler(TContext context)
        {
            Context = context;
        }

        #region Find
        /// <summary>
        /// Finds exactly one result matching the input predicate. If there are more or less, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is less or more than 1 result found matching the predicate</exception>
        public async Task<T> Find<T>(T predicate) where T : class, IEntity
        {            
            var query = await AbstractFind(predicate);

            var result = FindOneResult(query);

            return result;            
        }

        /// <summary>
        /// Finds one ore more results matching the input predicate. If there are 0, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="predicate">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is 0 results found matching the predicate</exception>
        public async Task<IEnumerable<T>> FindMultiple<T>(T predicate) where T : class, IEntity
        {            
            var query = await AbstractFind(predicate);

            var result = FindMultipleResults(query);

            return result;
            
        }

        protected abstract Task<IQueryable<T>> AbstractFind<T>(T predicate) where T : class, IEntity;

        #endregion

        #region Update and Retrieve
        /// <summary>
        /// Updates the inputed element in the database, and then retrieves and returns the updated version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element to update in database, and retrieve updated version of.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputed element's Id is 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it failes to changed the tracked state of the element to modified</exception>
        public async Task<T> UpdateAndRetrieve<T>(T element) where T : class, IEntity 
        {
            if (element.Id == 0)
                throw new ArgumentException($"I need an Id to figure out what to update", new ArgumentException("Id of predicate can not be 0"));

            var result = await VirtualUpdate(element);
            if (result == false)
                throw new TaskFailedException(TypeExtensions.GetMethodInfo<BaseHandler<TContext>>(nameof(UpdateAndRetrieve)), 
                    $"Task was suppose to update {nameof(element)} of type {typeof(T).FullName} in database, but failed to set state to modified");

            await Save();

            var updated = await Find(element);
            return updated;
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

        #region Add and Retrieve
        /// <summary>
        /// Addes the inputed element to the database, and then retrieves and returns the added version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element to add to the database, and retrieve the added version of.</param>
        /// <param name="tryRetrieveFirst">Wheather or not to attempt to retrieve using the inputed element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputed element's Id is not 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it failes to changed the tracked state of the element to added</exception>
        public async Task<T> AddAndRetrieve<T>(T element, bool tryRetrieveFirst = true) where T : class, IEntity 
        {
            if (element.Id != 0)
                throw new ArgumentException($"I need Id to be 0 to set it properly myself", new ArgumentException($"Id of predicate has to be 0"));

            if (tryRetrieveFirst) 
            {
                try
                {
                    var retrieve = await Find(element);
                    return retrieve;
                }
                catch (IncorrectCountException<T> ice)
                {
                    if (ice.ToMany) throw new Exception($"While attempting to retrive before adding, found more than one result matching {nameof(element)}", ice);
                }
            }            

            var result = await VirtualAdd(element);
            if (result == false)
                throw new TaskFailedException(TypeExtensions.GetMethodInfo<BaseHandler<TContext>>(nameof(AddAndRetrieve)),
                    $"Task was suppose to add {nameof(element)} of type {typeof(T).FullName} to database, but failed to set state to added");

            var added = await Find(element);
            return added;
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

        #region Add Multiple and Retrieve

        /// <summary>
        /// Adds a collection of elements to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="elements">The elements to add to the database.</param>
        /// <param name="tryRetrieveFirst">Wheather or not to attempt to retrieve using the inputed element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns>An ICollection of the added elements, after they have been freshly retrieved from the database</returns>
        public async Task<ICollection<T>> AddMultipleAndRetrieve<T>(ICollection<T> elements, bool tryRetrieveFirst = true) where T : class, IEntity 
        {
            var results = new List<T>();

            foreach (var element in elements)
            {
                results.Add(await AddAndRetrieve(element, tryRetrieveFirst));
            }

            return results;
        }

        #endregion
               
        #region Help Methods
        protected async Task Save()
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
        
        /// <summary>
        /// Retrieves exactly one result, matching the query input. If there are more or less, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query to run, and retrieve Entities via</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is less or more than 1 result found matching the query</exception>
        private T FindOneResult<T>(IQueryable<T> query) where T : class, IEntity
        {
            lock (ContextLock)
            {
                var result = query.ToList();
                if (result.Count == 1)
                    return result.First();
                if (result.Count > 1)
                    throw IncorrectEntityCountException<T>.Constructor(1, result.Count, true, result);
                throw IncorrectEntityCountException<T>.Constructor(1, result.Count, elements: new List<T>()); 
            }
        }
        /// <summary>
        /// Retrieves one or more results, matching the query input. If there are 0 results, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query">The query to run, and retrieve Entities via</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is 0 results found matching the query</exception>
        private ICollection<T> FindMultipleResults<T>(IQueryable<T> query) where T : class, IEntity
        {
            lock (ContextLock)
            {
                var result = query.ToList();
                if (result.Any())
                    return result;
                throw IncorrectEntityCountException<T>.Constructor(1, result.Count, elements: new List<T>()); 
            }
        }

        #endregion

        #region Obsolete

        #region Add

        [Obsolete("Use AddAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        public async Task<bool> Add<T>(T element, bool autoSave = true) where T : class, IEntity
        {
            if (element.Id != 0)
                throw new ArgumentException($"I need Id to be 0 to set it properly myself", new ArgumentException($"Id of predicate has to be 0"));

            var result = await VirtualAdd(element);

            if (autoSave)
                await Save();

            return result;
        }

        #endregion

        #region Update

        [Obsolete("Use UpdateAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        public async Task<bool> Update<T>(T element, bool autoSave = true) where T : class, IEntity
        {
            if (element.Id == 0)
                throw new Exception($"I need an Id to figure out what to update", new ArgumentException("Id of predicate can not be 0"));

            var result = await VirtualUpdate(element);

            return false;
        }



        #endregion

        #region AddMultiple

        [Obsolete("Use AddMultipleAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        public async Task<string> AddMultiple<T>(ICollection<T> elements) where T : class, IEntity
        {
            var added = await VirtualAddMultiple(elements);
            var result = GetAmountAdded(added);

            await Save();

            return result;
        }

        [Obsolete("Only used by AddMultiple which is also Obsolete")]
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

        [Obsolete("Only used by AddMultiple which is also Obsolete")]
        internal string GetAmountAdded(ICollection<bool> results)
        {
            return $"Added {results.Count(b => b)} out of {results.Count}.";
        }

        #endregion
    }
}

