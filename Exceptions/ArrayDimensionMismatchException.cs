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
        string _Message;
        public override String Message { get { return _Message; } }

        public ArrayDimensionMismatchException()
        {
            _Message = "";
        }

        public ArrayDimensionMismatchException(string message)
        {
            _Message = message;
        }
    }
}
