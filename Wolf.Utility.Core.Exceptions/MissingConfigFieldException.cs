using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Exceptions
{
    public class MissingConfigFieldException : BaseException
    {
        public MissingConfigFieldException(string message) : base(message)
        {
        }

        public MissingConfigFieldException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
