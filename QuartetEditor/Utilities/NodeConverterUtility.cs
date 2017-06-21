using QuartetEditor.Entities;
using QuartetEditor.Models;
using QuartetEditor.Utilities.ConverterExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuartetEditor.Utilities
{
    /// <summary>
    /// ノード変換ユーティリティ
    /// </summary>
    static public class NodeConverterUtility
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
            if (lineWidth - indent.GetSJISByte() < lineWidth / 2)
            {
                lineWidth = indent.GetSJISByte() + lineWidth / 2;
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

        #region ToTreeText

        /// <summary>
        /// 階層付きテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static public string ToTreeText(QuartetEditorDescription data, char headerChar)
        {
            StringBuilder export = new StringBuilder();
            int level = 1;

            foreach (var item in data.Node)
            {
                ToTreeText(item, export, headerChar, level);
            }
            return export.ToString();
        }

        /// <summary>
        /// ノードを階層付きテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToTreeText(QuartetEditorDescriptionItem item, StringBuilder result, char headerChar, int level)
        {
            // タイトルを変換
            string title = new string(headerChar, level);
            if (result.Length > 0)
            {
                // 先頭以降は改行後にタイトルを追加する
                title = Environment.NewLine + title;
            }

            if (item.Name.StartsWith(headerChar.ToString()))
            {
                // タイトルが制御文字で始まる場合は半角空白を先頭に入れる
                title += " ";
            }
            title += item.Name;
            result.Append(title);

            // コンテンツ変換
            string content = Environment.NewLine;
            using (var sr = new StringReader(item.Content))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith(headerChar.ToString()))
                    {
                        // 各行が制御文字で始まる場合は半角空白を先頭に入れる
                        content += " ";
                    }
                    content += line + Environment.NewLine;
                }
            }

            result.Append(content);

            foreach (var child in item.Children)
            {
                ToTreeText(child, result, headerChar, level + 1);
            }
        }
        #endregion ToTreeText

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
            export.AppendLine("<style type=\"text/css\" media=\"screen\">");
            {
                string css;
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                using (var resourceStream = assembly.GetManifestResourceStream("QuartetEditor.Assets.ExportHTMLStyle.css"))
                {
                    using (var resourceReader = new System.IO.StreamReader(resourceStream))
                    {
                        css = resourceReader.ReadToEnd();
                    }
                }
                export.AppendLine(css);
            }
            export.AppendLine("</style>");
            export.AppendLine(string.Format(@"<title>{0}</title>", HttpUtility.HtmlEncode(title)));
            export.AppendLine(@"</head>");
            export.AppendLine(@"<body>");

            var idDictionary = new Dictionary< QuartetEditorDescriptionItem, string>();

            export.AppendLine(@"<p>");
            export.AppendLine(@"<ul>");
            foreach (var item in data.Node)
            {
                ToHTMLList(item, ref idDictionary, export, level);
            }
            export.AppendLine(@"</ul>");
            export.AppendLine(@"</p>");

            foreach (var item in data.Node)
            {
                ToHTML(item, idDictionary, export, level);
            }

            export.AppendLine(@"</body>");
            export.AppendLine(@"</html>");
            return export.ToString();
        }

        /// <summary>
        /// ノードタイトルをHTMLのリストに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToHTMLList
            (
            QuartetEditorDescriptionItem item, 
            ref Dictionary<QuartetEditorDescriptionItem, string> idDictionary, 
            StringBuilder result, 
            int level
            )
        {
            string id = level.ToString() + "-" + Guid.NewGuid().ToString("N");
            idDictionary.Add(item, id);

            result.AppendLine(@"<li>" + string.Format("<a href=\"#{0}\">{1}</a>", HttpUtility.HtmlEncode(id), HttpUtility.HtmlEncode(item.Name)) + @"</li>");

            if (item.Children.Count() > 0)
            {
                result.AppendLine(@"<ul>");
                foreach (var child in item.Children)
                {
                    ToHTMLList(child, ref idDictionary, result, level + 1);
                }
                result.AppendLine(@"</ul>");
            }

            return;
        }

        /// <summary>
        /// ノードをHTMLデータに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToHTML(QuartetEditorDescriptionItem item, Dictionary<QuartetEditorDescriptionItem, string> idDictionary, StringBuilder result, int level)
        {
            // タイトルを変換
            int h = level > 6 ? 6 : level;

            result.AppendLine(string.Format("<h{0} id=\"{2}\">{1}</h{0}>", h, HttpUtility.HtmlEncode(item.Name), idDictionary[item]));

            // コンテンツ変換
            result.AppendLine(string.Format("<p class=\"Level{0}\">", level));
            System.IO.StringReader rs = new System.IO.StringReader(item.Content.TrimEnd());
            while (rs.Peek() > -1)
            {
                result.AppendLine(string.Format(@"{0}<br>", HttpUtility.HtmlEncode(rs.ReadLine())));
            }
            result.AppendLine(@"</p>");

            foreach (var child in item.Children)
            {
                ToHTML(child, idDictionary, result, level + 1);
            }
        }

        #endregion HTML

        #region FromTreeText

        /// <summary>
        /// 階層付きテキストファイルをノードに変換する
        /// </summary>
        /// <returns></returns>
        static public bool FromTreeText(string treeText, char headerChar, out QuartetEditorDescription QED)
        {
            QED = new QuartetEditorDescription();
            var nodes = new List<QuartetEditorDescriptionItem>();
            string lineFeed = FileUtility.GetLineFeedCode(treeText);

            // 行頭の制御文字がない場合はエラー
            int index = 0;
            if (!treeText.StartsWith(headerChar.ToString()))
            {
                index = treeText.IndexOf(lineFeed + headerChar.ToString());
                if (index == -1)
                {
                    return false;
                }
                index += lineFeed.Length;
            }

            treeText = treeText.SafeSubstring(index);
            FromTreeText(ref treeText,
                         headerChar,
                         lineFeed,
                         nodes,
                         1);

            QED.Node = nodes;

            // バージョンは1.0で固定
            QED.Version = 1.0;

            return true;
        }

        /// <summary>
        /// 階層付きテキストを分解します
        /// </summary>
        static private void FromTreeText(ref string treeText, char headerChar, string lineFeed, List<QuartetEditorDescriptionItem> Nodes, int level)
        {
            var headerMark = new string(headerChar, level);

            // 最初に行頭のマークが現れるまで読み飛ばし
            int index = 0;
            while (treeText.StartsWith(headerMark))
            {
                var titleStartPos = headerMark.Length;
                var titleEndPos = treeText.IndexOf(lineFeed);
                if (titleEndPos == -1)
                {
                    // 改行が見つからない場合、残り全部がタイトル
                    titleEndPos = treeText.Length;
                }
                var title = treeText.SubstringByIndex(titleStartPos, titleEndPos);
                if (title.StartsWith(" " + headerChar.ToString()))
                {
                    // タイトルが制御文字から始まるとき、空白でエスケープされている
                    title = title.SafeSubstring(1);
                }
                index = titleEndPos + lineFeed.Length;

                var content = "";
                var contentStartPos = index;
                if (treeText.SafeSubstring(contentStartPos).StartsWith(headerChar.ToString()) ||
                    treeText.Length <= contentStartPos)
                {
                    // コンテンツがなく、次のタイトルが現れている場合は何もしない
                }
                else
                {
                    // コンテンツは次に見つかる行頭の制御文字までの間
                    var contentEndPos = treeText.IndexOf(lineFeed + headerChar.ToString(), contentStartPos);
                    if (contentEndPos == -1)
                    {
                        // 次のタイトルが見つからないときは残りの文字列全部がコンテンツ
                        contentEndPos = treeText.Length;
                    }

                    content = treeText.SubstringByIndex(contentStartPos, contentEndPos);
                    content = content.Replace(lineFeed, Environment.NewLine);
                    content = DeleteEscapeSpace(content, headerChar);

                    index = contentEndPos + lineFeed.Length;
                }

                var node = new QuartetEditorDescriptionItem() { Name = title, Content = content };

                if (treeText.Length > index)
                {
                    treeText = treeText.SafeSubstring(index);
                }
                else
                {
                    treeText = "";
                }

                index = 0;

                // 子ノードの探索
                int nextChildTitleIndex = treeText.IndexOf(headerMark + headerChar.ToString(), index);
                if (nextChildTitleIndex != -1 &&
                    !treeText.SafeSubstring(0, nextChildTitleIndex).Contains(lineFeed + headerChar.ToString()))
                {
                    // 子階層のタイトルまでの間に別の階層のタイトルが見つからない場合
                    FromTreeText(ref treeText,
                                 headerChar,
                                 lineFeed,
                                 node.Children,
                                 level + 1);
                }

                Nodes.Add(node);
            }

        }

        /// <summary>
        /// 行頭エスケープ用の空白を削除します
        /// </summary>
        /// <param name="content"></param>
        /// <param name="headerChar"></param>
        /// <returns></returns>
        private static string DeleteEscapeSpace(string content, char headerChar)
        {
            string result = "";

            while (content.Length > 0)
            {
                // 1行分（改行文字を含む）を取り出す
                var lineEnd = content.IndexOf(Environment.NewLine);
                if (lineEnd == -1)
                {
                    // 改行コードが見つからない場合は残り全部が行
                    lineEnd = content.Length;
                }
                lineEnd += Environment.NewLine.Length;

                // １行分を取り出し
                var line = content.SubstringByIndex(0, lineEnd);

                // コンテンツ全体から１行分を削除
                content = content.SafeSubstring(lineEnd);

                // １行の先頭がエスケープされていたら空白を削除
                if (line.StartsWith(" " + headerChar.ToString()))
                {
                    line = line.SafeSubstring(1);
                }
                result += line;
            }

            return result;
        }

        #endregion FromTreeText
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

            /// <summary>
            /// 指定されたindexとindexの間の文字を取り出します
            /// </summary>
            /// <param name="s"></param>
            /// <param name="start"></param>
            /// <param name="end"></param>
            /// <returns></returns>
            public static string SubstringByIndex(this string s, int start, int end)
            {
                if (start >= end)
                {
                    return "";
                }

                return s.SafeSubstring(start, end - start);
            }

            /// <summary>
            /// 例外を出さないSubstring
            /// </summary>
            /// <param name="s"></param>
            /// <param name="startIndex"></param>
            /// <returns></returns>
            public static string SafeSubstring(this string s, int startIndex)
            {
                if (startIndex < 0)
                {
                    startIndex = 0;
                }

                if (startIndex > s.Length - 1)
                {
                    return "";
                }

                return s.Substring(startIndex);

            }

            /// <summary>
            /// 例外を出さないSubstring
            /// </summary>
            /// <param name="s"></param>
            /// <param name="startIndex"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public static string SafeSubstring(this string s, int startIndex, int length)
            {
                if (startIndex < 0)
                {
                    startIndex = 0;
                }

                if (length < 1)
                {
                    return "";
                }

                if (startIndex > s.Length - 1)
                {
                    return "";
                }

                if (startIndex + length > s.Length)
                {
                    return s;
                }

                return s.Substring(startIndex, length);
            }

        }
        #endregion Text
    }
}
