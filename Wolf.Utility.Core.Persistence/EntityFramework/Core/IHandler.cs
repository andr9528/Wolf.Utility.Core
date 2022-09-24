using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wolf.Utility.Core.Persistence.Core;

namespace Wolf.Utility.Core.Persistence.EntityFramework.Core
{
    public interface IHandler<TEntity, TSearchable> where TEntity : class, IEntity
        where TSearchable : class, ISearchableEntity
    {
        /// <summary>
        /// Finds exactly one result matching the input predicate. If there are more or less, throws an exception.
        /// </summary>
        /// <param name="entity">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is less or more than 1 result found matching the predicate</exception>
        Task<TEntity> Find(TSearchable entity);

        /// <summary>
        /// Finds one ore more results matching the input predicate. If there are 0, throws an exception.
        /// </summary>
        /// <param name="entity">A version of the type to search for, with some of its properties set to search parameters</param>
        /// <returns></returns>
        /// <exception cref="Wolf.Utility.Core.Exceptions.IncorrectCountException{T}">Thrown when there is 0 results found matching the predicate</exception>
        Task<IEnumerable<TEntity>> FindMultiple(TSearchable entity);

        /// <summary>
        /// Updates the inputted element in the database, and then retrieves and returns the updated version.
        /// </summary>
        /// <param name="entity">The element to update in database, and retrieve updated version of.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputted element's Id is 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it fails to changed the tracked state of the element to modified</exception>
        Task<TEntity> UpdateAndRetrieve(TEntity entity);

        /// <summary>
        /// Updates the inputted elements in the database, and then retrieves and returns the updated versions.
        /// </summary>
        /// <param name="entities">The elements to update in database, and retrieve updated versions of.</param>
        /// <returns></returns>
        Task<ICollection<TEntity>> UpdateMultipleAndRetrieve(ICollection<TEntity> entities);

        /// <summary>
        /// Deletes an inputted entity from the backing database.
        /// </summary>
        /// <param name="entity">The entity to delete from the database.</param>
        /// <returns>True is it was deleted, otherwise false.</returns>
        /// <exception cref="ArgumentException">Thrown if the Id of <paramref name="entity"/> is 0.</exception>
        Task<bool> Delete(TEntity entity);

        /// <summary>
        /// If needed Finds the specified entity, and deletes it. Otherwise simply deletes it. 
        /// </summary>
        /// <param name="id"></param>
        /// <returns>True is it was deleted, otherwise false.</returns>
        /// <exception cref="ArgumentException">Thrown if the <paramref name="id"/> is 0.</exception> 
        Task<bool> Delete(int id);

        /// <summary>
        /// Adds the inputted element to the database, and then retrieves and returns the added version.
        /// </summary>
        /// <param name="element">The element to add to the database, and retrieve the added version of.</param>
        /// <param name="tryRetrieveFirst">Whether or not to attempt to retrieve using the inputted element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Thrown when the inputted element's Id is not 0</exception>
        /// <exception cref="Wolf.Utility.Core.Exceptions.TaskFailedException">Thrown when it fails to changed the tracked state of the element to added</exception>
        Task<TEntity> AddAndRetrieve(TEntity element, bool tryRetrieveFirst = true);

        /// <summary>
        /// Adds a collection of elements to the database
        /// </summary>
        /// <param name="entities">The elements to add to the database.</param>
        /// <param name="tryRetrieveFirst">Whether or not to attempt to retrieve using the inputted element, before adding, and then retrieving. 
        /// Lowers duplicate entities, in theory</param>
        /// <returns>An ICollection of the added elements, after they have been freshly retrieved from the database</returns>
        Task<ICollection<TEntity>> AddMultipleAndRetrieve(ICollection<TEntity> entities, bool tryRetrieveFirst = true);
    }
}