﻿using Newtonsoft.Json;
using QuartetEditor.Utilities.CharacterCode;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuartetEditor.Entities;

namespace QuartetEditor.Utilities
{
    /// <summary>
    /// ファイル操作ユーティリティ
    /// </summary>
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

            try
            {
                target = JsonConvert.DeserializeObject<T>(json);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// エンコードを自動判別してファイルを読み込みます
        /// </summary>
        /// <param name="fileName">ファイル名</param>
        /// <param name="text">読み込んだテキストデータ</param>
        /// <returns>読み込みに失敗、またはテキストファイルではないときfalse</returns>
        public static bool LoadTextByAnyEncoding(string fileName, out string text)
        {
            text = default(string);

            if (!File.Exists(fileName))
            {
                return false;
            }

            System.IO.FileInfo file = new FileInfo(fileName);
            using (FileReader reader = new FileReader(file))
            {
                CharCode c = reader.Read(file);

                if (!(c is CharCode.Text))
                {
                    // テキストファイルじゃない
                    return false;
                }

                text = reader.Text;
            }
            return true;
        }

        /// <summary>
        /// テキストに含まれる改行コードを取得します
        /// 改行コードが混在している場合の動作は未保証
        /// CRには未対応（どうせないし）
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetLineFeedCode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Environment.NewLine;
            }

            if (text.Contains("\r\n"))
            {
                // CR+LF
                return "\r\n";
            }
            // LF
            return "\n";

        }
    }
}
