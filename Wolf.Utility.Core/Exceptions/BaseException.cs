using System;
using System.Collections.Generic;
using System.Text;

namespace Wolf.Utility.Core.Exceptions
{
    public abstract class BaseException : Exception
    {
        protected BaseException(string message) : base(message)
        {

        }

        protected BaseException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
