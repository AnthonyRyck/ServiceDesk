using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceDeskToolsCore.Exceptions
{
    public class GroupActiveDirectoryNotExistException : Exception
    {
        public GroupActiveDirectoryNotExistException()
            : base()
        {

        }

        public GroupActiveDirectoryNotExistException(string message)
            : base(message)
        {

        }

        public GroupActiveDirectoryNotExistException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}
