using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Mvc;

using Wolf.Utility.Core.Exceptions;
using Wolf.Utility.Core.Logging;
using Wolf.Utility.Core.Persistence.Core;
using Wolf.Utility.Core.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Core.Web
{
    /// <summary>
    /// For use with Api controllers, working with entity framework core. includes CRUD for the entity defined in TEntity.
    /// </summary>
    /// <typeparam name="TEntity">Entity that CRUD should be implemented for.</typeparam>
    public abstract class AdvancedController<TEntity> : ControllerBase, IAdvancedController<TEntity> where TEntity : class, IEntity
    {
        protected enum MessageType
        {
            Null, Must, Cannot, Unknown
        }

        protected enum CallType
        {
            Null, Add, Update, Delete
        }

        protected IHandler Handler { get; }
        public ILoggerManager Logger { get; }

        protected AdvancedController(IHandler handler, ILoggerManager logger)
        {
            Handler = handler;
            Logger = logger;
        }
        [HttpGet]
        public virtual async Task<ActionResult<IEnumerable<TEntity>>> Get([FromQuery]TEntity entity) 
        {
            try
            {
                var result = await Handler.FindMultiple(entity);

                return new OkObjectResult(result);
            }
            catch (IncorrectCountException<TEntity> ice)
            {
                Logger.LogError($"{ice}; Stacktrace: {ice.StackTrace}");
                return BadRequest(new Exception($"Failed to find the correct amount when searching for entities. " +
                    $"Found: {ice.ActualCount}; Expected {ice.ExpectedCount}", ice));
            }
        }
        [HttpPut]
        public virtual async Task<ActionResult<TEntity>> Put(TEntity entity)
        {
            try
            {
                var result = await Handler.UpdateAndRetrieve(entity);

                return new OkObjectResult(result);
            }
            catch (ArgumentException ae)
            {
                Logger.LogError($"{ae}; Stacktrace: {ae.StackTrace}");
                return BadRequest(new Exception($"Expected the inputed entity to have an Id that was not 0, but it was 0.", ae));
            }
            catch (IncorrectCountException<TEntity> ice)
            {
                Logger.LogError($"{ice}; Stacktrace: {ice.StackTrace}");
                return BadRequest(new Exception($"Failed to find the correct amount when searching for the updated entity. " +
                    $"Found: {ice.ActualCount}; Expected {ice.ExpectedCount}", ice));
            }
        }
        [HttpPost]
        public virtual async Task<ActionResult<TEntity>> Post(TEntity entity)
        {
            try
            {
                var result = await Handler.AddAndRetrieve(entity);

                return new OkObjectResult(result);
            }
            catch (ArgumentException ae)
            {
                Logger.LogError($"{ae}; Stacktrace: {ae.StackTrace}");
                return BadRequest(new Exception($"Expected the inputed entity to have an Id that was 0, but it was not 0.", ae));
            }
            catch (IncorrectCountException<TEntity> ice)
            {
                Logger.LogError($"{ice}; Stacktrace: {ice.StackTrace}");
                return BadRequest(new Exception($"Failed to find the correct amount when searching for the added entity. " +
                    $"Found: {ice.ActualCount}; Expected {ice.ExpectedCount}", ice));
            }
        }
        [HttpDelete]
        public virtual async Task<IActionResult> Delete(TEntity entity)
        {
            try
            {
                var result = await Handler.Delete(entity);

                if (result) return new OkResult();
                return BadRequest($"Failed to delete entity from backing database.");
            }
            catch (ArgumentException ae)
            {
                Logger.LogError($"{ae}; Stacktrace: {ae.StackTrace}");
                return BadRequest(new Exception($"Id of entity was 0. To delete an entity it needs to have the id of the entity to delete. " +
                    $"Retrieve it from the database, then call the delete with the retrieved instance.", ae));
            }
        }

        protected ObjectResult CreateReturnMessage(string message)
        {
            return new ObjectResult(message);
        }

        protected ObjectResult CreateReturnMessage((MessageType, CallType) preConstructedMessage)
        {
            var (messageType, callType) = preConstructedMessage;

            switch (messageType)
            {
                case MessageType.Null:
                    throw new ArgumentNullException(nameof(messageType),$"Message type was Null");
                case MessageType.Must:
                    return new ObjectResult($"The Id of imputed {nameof(TEntity)} MUST be {default(int)}, otherwise it will break when attempting to {callType}");
                case MessageType.Cannot:
                    return new ObjectResult($"The Id of imputed {nameof(TEntity)} CANNOT be {default(int)}, because then it is impossible to figure out what to {callType}.");
                case MessageType.Unknown:
                    return new ObjectResult($"Something went wrong attempting to {callType} a {nameof(TEntity)} in the database.");
                default:
                    throw new ArgumentOutOfRangeException(nameof(messageType), messageType, null);
            }
        }
    }
}
