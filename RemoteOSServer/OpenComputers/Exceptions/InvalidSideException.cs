using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteOS.OpenComputers.Exceptions
{
    public class InvalidSideException : Exception
    {
        public InvalidSideException() : base() { }
        public InvalidSideException(string message) : base(message) { }
    }
}
