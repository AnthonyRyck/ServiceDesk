using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Exceptions
{
    public class UserExistInGroupException : Exception
    {
        public UserExistInGroupException()
            : base()
        {

        }

        public UserExistInGroupException(string message)
            : base(message)
        {

        }

        public UserExistInGroupException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
