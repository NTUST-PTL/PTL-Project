using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.Windows.Forms;
using System.IO;

namespace PTL.Tools.FileOperation
{
    public class XML
    {
        /// <summary>
        /// 將一物件序列化並以XML格式存成文字檔。
        /// </summary>
        /// <param name="obj">將序列化的物件</param>
        /// <param name="fileName">檔案儲存路徑</param>
        /// <param name="fileMode">檔案儲存模式</param>
        public static void WriteXMLFile(Object obj, string fileName, FileMode fileMode)
        {

            XmlSerializer n = new XmlSerializer(obj.GetType());
            FileStream k;

            if (fileMode == FileMode.Create)
            {
                k = new FileStream(fileName, fileMode);
                n.Serialize(k, obj);
                k.Close();
            }
            else
            {
                try
                {
                    k = new FileStream(fileName, fileMode);
                    n.Serialize(k, obj);
                    k.Close();
                }
                catch (IOException ex)
                {
                    k = new FileStream(fileName, FileMode.Create);
                    n.Serialize(k, obj);
                    k.Close();
                    string s1 = ex.Message;
                }
            }
        }

        /// <summary>
        /// 從XML格式文字檔讀取資訊並建立對應物件。
        /// </summary>
        /// <typeparam name="T">反序列化需提供對應的類別</typeparam>
        /// <param name="name">讀取文件之檔案路徑</param>
        /// <returns></returns>
        public static T ReadXMLFile<T>(string name)
        {
            T Output;
            try
            {
                XmlSerializer n = new XmlSerializer(typeof(T));
                FileStream k;
                k = new FileStream(name, FileMode.Open);
                Output = (T)n.Deserialize(k);
                k.Close();
                return Output;
            }
            catch (IOException ex)
            {
                var result = MessageBox.Show("檔案不存在", "注意");
                string s1 = ex.Message;
            }
            catch (InvalidOperationException  ex)
            {
                var result = MessageBox.Show(name+"\n"+"檔案有錯", "注意");
                string s1 = ex.Message;
            }
            return (T)(new object());
        }
    }
}
