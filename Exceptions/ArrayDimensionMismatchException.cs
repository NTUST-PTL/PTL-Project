using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace PTL.Exceptions
{
    public class ArrayDimensionMismatchException : Exception
    {
        String Message;

        public ArrayDimensionMismatchException()
        {
            Message = "";
        }

        public ArrayDimensionMismatchException(string message)
        {
            Message = message;
        }
    }
}
