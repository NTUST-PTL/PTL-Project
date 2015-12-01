using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PTL.FileOperation
{
    public static class StringFormat
    {
        public static string StringStr(string value)
        {
            string str = String.Format("{0,16}", value);
            return str;
        }
        public static string StringDouble(double value)
        {
            string str = String.Format("{0,16}", String.Format("{0:0.000}", value));
            return str;
        }
        public static string StringDouble(double value,int tdecimal,int space)
        {
            string strdecimal = StringDecimalFormate(tdecimal);
            string strspace = "{0," + Convert.ToString(space) + "}";

            string str = String.Format(strspace, String.Format("{0:0.000}", value));
            return str;
        }
        public static string StringDouble(double value, int tdecimal)
        {
            string strdecimal = StringDecimalFormate(tdecimal);

            string str = String.Format("{0,16}", String.Format(strdecimal, value));
            return str;
        }
        public static string StringInt(int value)
        {
            string str = String.Format("{0,16}", String.Format("{0:0}", value));
            return str;
        }

        public static string StringDecimalFormate(int tdecimal)
        {
            string strdecimal = "{0:0.";
            for (short k = 0; k < tdecimal; k++)
                strdecimal = strdecimal + "0";

            strdecimal = strdecimal + "}";

            return strdecimal;
        }
    }
}
