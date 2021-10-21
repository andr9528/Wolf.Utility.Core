using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework.Core
{
    public interface IHandler
    {
        /// <summary>
        /// Finds exactly one result matching the input predicate. If there are more or less, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is less or more than 1 result found matching the predicate</exception>
        Task<T> Find<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// Finds one ore more results matching the input predicate. If there are 0, throws an exception.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is 0 results found matching the predicate</exception>
        Task<IEnumerable<T>> FindMultiple<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// Updates the inputed element in the database, and then retrieves and returns the updated version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The element to update in database, and retrieve updated version of.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputed element's Id is 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it failes to changed the tracked state of the element to modified>
        Task<T> UpdateAndRetrieve<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// Deletes an inputed entity fro mthe backing database.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity to delete from the database.</param>
        /// <returns>True is it was deleted, otherwise false.</returns>
        /// <exception cref="ArgumentException">Thrown if the Id of <paramref name="entity"/> is 0.</exception>
        Task<bool> Delete<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// Addes the inputed element to the database, and then retrieves and returns the added version.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element">The element to add to the database, and retrieve the added version of.</param>
        /// <param name="tryRetrieveFirst">Wheather or not to attempt to retrieve using the inputed element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputed element's Id is not 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it failes to changed the tracked state of the element to added>
        Task<T> AddAndRetrieve<T>(T element, bool tryRetrieveFirst = true) where T : class, IEntity;

        /// <summary>
        /// Adds a collection of elements to the database
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities">The elements to add to the database.</param>
        /// <param name="tryRetrieveFirst">Wheather or not to attempt to retrieve using the inputed element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns>An ICollection of the added elements, after they have been freshly retrieved from the database</returns>
        Task<ICollection<T>> AddMultipleAndRetrieve<T>(ICollection<T> entities, bool tryRetrieveFirst = true) where T : class, IEntity;

        #region Obsolete
        [Obsolete("Use AddMultipleAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        Task<string> AddMultiple<T>(ICollection<T> elements) where T : class, IEntity;

        [Obsolete("Use AddAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        Task<bool> Add<T>(T element, bool autoSave = true) where T : class, IEntity;

        [Obsolete("Use UpdateAndRetrieve instead. As IEntity makes use of Version History, the most recent version is needed for further changes.")]
        Task<bool> Update<T>(T element, bool autoSave = true) where T : class, IEntity;
        #endregion
    }
}
