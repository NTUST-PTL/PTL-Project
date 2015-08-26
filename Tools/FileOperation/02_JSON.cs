using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace PTL.Tools.FileOperation
{
    /// <summary>
    /// 具備將物件序列化並以Json格式存成文字檔的靜態方法。
    /// </summary>
    public class Json
    {
        /// <summary>
        /// 將一物件序列化並以Json格式存成文字檔。
        /// </summary>
        /// <param name="obj">將序列化的物件</param>
        /// <param name="fileName">檔案儲存路徑</param>
        public static void CreateJsonFile(object obj, String fileName)
        {
            CreateJsonFile(obj, fileName, Formatting.Indented);
        }

        /// <summary>
        /// 將一物件序列化並以Json格式存成文字檔。
        /// </summary>
        /// <param name="obj">將序列化的物件</param>
        /// <param name="fileName">檔案儲存路徑</param>
        /// <param name="formatting">是否換行
        /// ：Indented，輸出文件將自動換行，將具有較高的可讀型；
        /// None，輸出文件不會自動換行，輸出檔案大小大幅減少</param>
        public static void CreateJsonFile(object obj, String fileName, Formatting formatting)
        {
            if (fileName.Split('.').Last().ToLower() != "json")
                fileName += ".json";
            FileInfo fileInfo = new FileInfo(fileName);
            StreamWriter sw = fileInfo.CreateText();

            string jsonContent = JsonConvert.SerializeObject(obj, formatting);
            sw.WriteLine(jsonContent);

            sw.Flush();
            sw.Close();
        }

        /// <summary>
        /// 從Json格式文字檔讀取資訊並建立對應物件。
        /// </summary>
        /// <typeparam name="T">反序列化需提供對應的類別</typeparam>
        /// <param name="fileName">讀取文件之檔案路徑</param>
        /// <returns>回傳型別 T 的物件</returns>
        public static T ReadJsonFile<T>(String fileName)
        {
            StreamReader sr = new StreamReader(fileName);
            T obj = JsonConvert.DeserializeObject < T > (sr.ReadToEnd());
            sr.Close();
            return obj;
        }
    }

}
