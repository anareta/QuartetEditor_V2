using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Utilities
{
    public static class FileUtility
    {
        /// <summary>
        /// テキストをファイルに保存します
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static bool SaveText(string fileName, string text, Encoding encode)
        {
            try
            {
                using (var sw = new StreamWriter(fileName, false, encode))
                {
                    sw.Write(text);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Jsonにシリアライズして保存する
        /// </summary>
        public static bool SaveJsonObject<T>(string fileName, T target)
        {
            string json = JsonConvert.SerializeObject(target, Formatting.Indented);

            if (!FileUtility.SaveText(fileName, json, Encoding.UTF8))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// テキストを読み込みます
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="text"></param>
        /// <param name="encode"></param>
        /// <returns></returns>
        public static bool LoadText(string fileName, out string text, Encoding encode)
        {
            text = default(string);
            try
            {
                using (var sr = new StreamReader(fileName, encode))
                {
                    text = sr.ReadToEnd();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Jsonからデシリアライズして読み込む
        /// </summary>
        public static bool LoadJsonObject<T>(string fileName, out T target)
        {
            target = default(T);
            string json;

            if (!FileUtility.LoadText(fileName, out json, Encoding.UTF8))
            {
                return false;
            }

            target = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
    }
}
