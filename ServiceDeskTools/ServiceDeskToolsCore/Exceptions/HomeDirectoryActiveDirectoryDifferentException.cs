using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Exceptions
{
    public class HomeDirectoryActiveDirectoryDifferentException : Exception
    {
        public HomeDirectoryActiveDirectoryDifferentException()
            : base()
        {

        }

        public HomeDirectoryActiveDirectoryDifferentException(string message)
            : base(message)
        {

        }

        public HomeDirectoryActiveDirectoryDifferentException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
