using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wolf.Utility.Core.Exceptions
{
    public class ImpossibleException : BaseException
    {
        public ImpossibleException(string message) : base(message)
        {
        }
    }
}
