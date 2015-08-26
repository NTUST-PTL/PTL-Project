using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PTL.Tools.BinaryExtentionMethods
{
    public static class BinaryReaderExtentions
    {
        public static Tuple<bool, double> TryReadDouble(this BinaryReader binaryReader)
        {
            bool succeed = true;
            double result = 0;
            try 
	        {
                result = binaryReader.ReadDouble();
	        }
	        catch (Exception)
	        {
		        succeed = false;
	        }
            return new Tuple<bool, double>(succeed, result);
        }

        public static Tuple<bool, UInt16> TryReadUInt16(this BinaryReader binaryReader)
        {
            bool succeed = true;
            UInt16 result = 0;
            try
            {
                result = binaryReader.ReadUInt16();
            }
            catch (Exception)
            {
                succeed = false;
            }
            return new Tuple<bool, UInt16>(succeed, result);
        }

        public static Tuple<bool, float> TryReadSingle(this BinaryReader binaryReader)
        {
            bool succeed = true;
            float result = 0;
            try
            {
                result = binaryReader.ReadSingle();
            }
            catch (Exception)
            {
                succeed = false;
            }
            return new Tuple<bool, float>(succeed, result);
        }
    }   
}
