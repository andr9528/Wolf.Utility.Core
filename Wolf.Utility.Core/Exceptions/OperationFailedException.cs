using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Exceptions
{
    public class OperationFailedException : BaseException
    {
        public OperationFailedException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public OperationFailedException(string message) : base(message)
        {

        }
    }
}
