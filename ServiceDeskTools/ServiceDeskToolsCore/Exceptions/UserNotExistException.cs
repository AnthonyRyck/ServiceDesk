using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Exceptions
{
    public class UserNotExistException : Exception
    {
        public UserNotExistException()
            : base()
        {

        }

        public UserNotExistException(string message)
            : base(message)
        {

        }

        public UserNotExistException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
