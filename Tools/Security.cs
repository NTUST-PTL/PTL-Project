using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;

namespace PTL.Tools
{
    public static class Security
    {
        //private static string _Key = "1ab6c8ae";
        //private static string _IV = "b32cc529";
        private static string _Key = "5744d4ab";
        private static string _IV = "b40829ca";

        //加密金鑰(8個英文字) 
        public static string Key
        {
            set
            {
                _Key = value.Length == 8 ? value : "PPTL7598";
            }
        }

        // 初始化向量(8個英文字) 
        public static string IV
        {
            set
            {
                _IV = value.Length == 8 ? value : "TPTL7598";
            }
        }

        public static void ChangeBase(string newKey, string newIV)
        {
            Key = newKey;
            IV = newIV;
        }

        public static string Encrypt(string value)
        {
            return Encrypt(value, _Key, _IV);
        }

        public static string Decrypt(string value)
        {
            return Decrypt(value, _Key, _IV);
        }

        private static string Encrypt(string pToEncrypt, string sKey, string sIV)
        {
            StringBuilder ret = new StringBuilder();
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                //將字元轉換為Byte 
                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                //設定加密金鑰(轉為Byte) 
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //設定初始化向量(轉為Byte) 
                des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(inputByteArray, 0, inputByteArray.Length);
                        cs.FlushFinalBlock();
                    }
                    //輸出資料 
                    foreach (byte b in ms.ToArray())
                        ret.AppendFormat("{0:X2}", b);
                }
            }
            //回傳 
            return ret.ToString();
        }

        private static string Decrypt(string pToDecrypt, string sKey, string sIV)
        {
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {

                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                //反轉 
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }
                //設定加密金鑰(轉為Byte) 
                des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
                //設定初始化向量(轉為Byte) 
                des.IV = ASCIIEncoding.ASCII.GetBytes(sIV);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        //例外處理 
                        try
                        {
                            cs.Write(inputByteArray, 0, inputByteArray.Length);
                            cs.FlushFinalBlock();
                            //輸出資料 
                            return System.Text.Encoding.Default.GetString(ms.ToArray());
                        }
                        catch (CryptographicException)
                        {
                            //若金鑰或向量錯誤，傳回N/A 
                            return "N/A";
                        }
                    }
                }
            }
        }

        public static bool ValidateString(string EnString, string FoString)
        {
            //呼叫Decrypt解密 
            //判斷是否相符 
            //回傳結果 
            return Decrypt(EnString, _Key, _IV) == FoString.ToString() ? true : false;
        }

    }
}
