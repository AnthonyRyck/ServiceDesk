using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Exceptions
{
    public class UserNotInGroupException : Exception
    {
        public UserNotInGroupException()
            : base()
        {

        }

        public UserNotInGroupException(string message)
            : base(message)
        {

        }

        public UserNotInGroupException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
