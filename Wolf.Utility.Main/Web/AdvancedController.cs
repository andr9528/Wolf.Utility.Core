using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Wolf.Utility.Main.Persistence.Core;
using Wolf.Utility.Main.Persistence.EntityFramework.Core;

namespace Wolf.Utility.Main.Web
{
    public abstract class AdvancedController<TEntity, TUser> : ControllerBase, IAdvancedController<TEntity, TUser> where TEntity : class, IEntity where TUser : class, IEntity
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

        protected AdvancedController(IHandler handler)
        {
            Handler = handler;
        }

        public abstract Task<ActionResult<IEnumerable<TEntity>>> Get(TEntity entity);
        public abstract Task<IActionResult> Put((TEntity, TUser) data);
        public abstract Task<IActionResult> Post((TEntity, TUser) data);
        public abstract Task<IActionResult> Delete((TEntity, TUser) data);

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
