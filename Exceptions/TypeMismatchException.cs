using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PTL.Exceptions
{
    public class TypeMismatchException : Exception
    {
        public new String Message { get; set; }

        public TypeMismatchException()
        {
            Message = "";
        }

        public TypeMismatchException(string message)
        {
            Message = message;
        }
    }
}
