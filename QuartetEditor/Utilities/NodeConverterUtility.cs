using QuartetEditor.Entities;
using QuartetEditor.Models;
using QuartetEditor.Utilities.ConverterExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuartetEditor.Utilities
{
    /// <summary>
    /// ノード変換ユーティリティ
    /// </summary>
    static class NodeConverterUtility
    {
        #region Text

        /// <summary>
        /// QEDデータ構造をテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static public string ToText(QuartetEditorDescription data, ExportSettingModel setting)
        {
            StringBuilder export = new StringBuilder();
            int level = 0;
            if (setting.EnableLineWrap)
            {
                // 折り返しありの場合
                foreach (var item in data.Node)
                {
                    ToTextWithLineWrap(item, export, setting, level);
                }
                return export.ToString();
            }
            else
            {
                // 折り返しなしの場合
                foreach (var item in data.Node)
                {
                    ToText(item, export, level);
                }
                return export.ToString();
            }
        }

        /// <summary>
        /// ノードをテキストデータに変換する（折り返しあり）
        /// </summary>
        /// <returns></returns>
        static private void ToTextWithLineWrap(QuartetEditorDescriptionItem item, StringBuilder result, ExportSettingModel setting, int level)
        {
            // タイトルを変換
            result.AppendLine(new string(' ', level * 2) + "【" + item.Name + "】");

            // コンテンツ変換
            string indent = new string(' ', (level + 1) * 2);
            int lineWidth = setting.LineWrap;
            if (lineWidth - indent.GetSJISByte() < lineWidth/2)
            {
                lineWidth = indent.GetSJISByte() + lineWidth/2;
            }

            int index = 0;

            // 改行コードが２文字だと都合が悪いので一時的に置き換え
            string content = item.Content.Trim(Environment.NewLine.ToCharArray()).Replace(Environment.NewLine, "\r");
            while (index < content.Length)
            {
                var line = new StringBuilder(indent);
                while (line.GetSJISByte() < lineWidth && index < content.Length)
                {
                    if (content.Substring(index, 1) == "\r")
                    {
                        // 追加するのが改行コードだった場合、空白を入れなおす
                        ++index;
                        break;
                    }

                    line.Append(content.Substring(index, 1));
                    ++index;
                }
                result.AppendLine(line.ToString());
            }
            result.LineBreak();

            foreach (var child in item.Children)
            {
                ToTextWithLineWrap(child, result, setting, level + 1);
            }
        }

        /// <summary>
        /// ノードをテキストデータに変換する（折り返しなし）
        /// </summary>
        /// <returns></returns>
        static private void ToText(QuartetEditorDescriptionItem item, StringBuilder result, int level)
        {
            // タイトルを変換
            result.AppendLine(new string(' ', level * 2) + "【" + item.Name + "】");

            // コンテンツ変換
            string indent = new string(' ', (level + 1) * 2);

            int index = 0;

            // 改行コードが２文字だと都合が悪いので一時的に置き換え
            string content = item.Content.Trim(Environment.NewLine.ToCharArray()).Replace(Environment.NewLine, "\r");
            while (index < content.Length)
            {
                var line = new StringBuilder(indent);
                while (index < content.Length)
                {
                    if (content.Substring(index, 1) == "\r")
                    {
                        // 追加するのが改行コードだった場合、空白を入れなおす
                        ++index;
                        break;
                    }

                    line.Append(content.Substring(index, 1));
                    ++index;
                }
                result.AppendLine(line.ToString());
            }
            result.LineBreak();

            foreach (var child in item.Children)
            {
                ToText(child, result, level + 1);
            }
        }
        #endregion Text

        #region TreeText

        /// <summary>
        /// 階層付きテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static public string ToTreeText(QuartetEditorDescription data)
        {
            StringBuilder export = new StringBuilder();
            int level = 1;
            foreach (var item in data.Node)
            {
                ToTreeText(item, export, level);
            }
            return export.ToString();
        }

        /// <summary>
        /// ノードを階層付きテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToTreeText(QuartetEditorDescriptionItem item, StringBuilder result, int level)
        {
            // タイトルを変換
            string title = new string('.', level);
            if (item.Name.StartsWith("."))
            {
                // タイトルがピリオドで始まる場合は半角空白を先頭に入れる
                title += " ";
            }
            title += item.Name;
            result.AppendLine(title);

            // コンテンツ変換
            string content = "";
            if (item.Content.StartsWith("."))
            {
                // 本文がピリオドで始まる場合は半角空白を先頭に入れる
                content += " ";
            }
            content += item.Content.ToString();
            result.AppendLine(content);

            foreach (var child in item.Children)
            {
                ToTreeText(child, result, level + 1);
            }
        }
        #endregion TreeText

        #region HTML

        /// <summary>
        /// HTMLデータに変換する
        /// </summary>
        /// <returns></returns>
        static public string ToHTML(QuartetEditorDescription data, string title)
        {
            StringBuilder export = new StringBuilder();
            int level = 1;

            export.AppendLine(@"<!DOCTYPE HTML>");
            export.AppendLine("<html lang=\"ja\">");
            export.AppendLine(@"<head>");
            export.AppendLine("<meta charset=\"utf-8\">");
            export.AppendLine(string.Format(@"<title>{0}</title>", HttpUtility.HtmlEncode(title)));
            export.AppendLine(@"</head>");
            export.AppendLine(@"<body>");

            foreach (var item in data.Node)
            {
                ToHTML(item, export, level);
            }

            export.AppendLine(@"</body>");
            export.AppendLine(@"</html>");
            return export.ToString();
        }

        /// <summary>
        /// ノードをHTMLデータに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToHTML(QuartetEditorDescriptionItem item, StringBuilder result, int level)
        {
            // タイトルを変換
            int h = level > 6 ? 6 : level;

            result.AppendLine(string.Format(@"<h{0}>{1}</h{0}>", h, HttpUtility.HtmlEncode(item.Name)));

            // コンテンツ変換
            result.AppendLine(string.Format("<p class=\"Level{0}\">", level));
            System.IO.StringReader rs = new System.IO.StringReader(item.Content);
            while (rs.Peek() > -1)
            {
                result.AppendLine(string.Format(@"{0}<br>", HttpUtility.HtmlEncode(rs.ReadLine())));
            }
            result.AppendLine(@"</p>");

            foreach (var child in item.Children)
            {
                ToHTML(child, result, level + 1);
            }
        }
        #endregion HTML
    }

    namespace ConverterExtensions
    {
        #region Text
        /// <summary>
        /// コンバーター用拡張メソッド
        /// </summary>
        static class ConverterExtensions
        {
            /// <summary>
            /// 改行する
            /// </summary>
            /// <param name="s"></param>
            /// <param name="times"></param>
            /// <returns></returns>
            public static StringBuilder LineBreak(this StringBuilder s, int times = 1)
            {
                for (int i = 0; i < times; i++)
                {
                    s.Append(Environment.NewLine);
                }
                return s;
            }

            /// <summary>
            /// SJISに変換したときのバイト数を取得
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static int GetSJISByte(this StringBuilder s)
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                return sjisEnc.GetByteCount(s.ToString());
            }

            /// <summary>
            /// SJISに変換したときのバイト数を取得
            /// </summary>
            /// <param name="s"></param>
            /// <returns></returns>
            public static int GetSJISByte(this string s)
            {
                Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");
                return sjisEnc.GetByteCount(s);
            }
        }
        #endregion Text
    }
}
