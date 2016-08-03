using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PTL.Exceptions
{
    public class ArraySizeMismatchException : Exception
    {
        public new String Message { get; set; }

        public ArraySizeMismatchException()
        {
            Message = "";
        }

        public ArraySizeMismatchException(string message)
        {
            Message = message;
        }
    }
}
