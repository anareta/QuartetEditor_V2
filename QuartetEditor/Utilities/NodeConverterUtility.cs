using QuartetEditor.Entities;
using QuartetEditor.Utilities.ConverterExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Utilities
{
    /// <summary>
    /// ノード変換ユーティリティ
    /// </summary>
    static class NodeConverterUtility
    {
        #region Text

        /// <summary>
        /// テキストの折り返し幅（SJISバイト数）
        /// </summary>
        static readonly int LinefeedWidth = 80;

        /// <summary>
        /// ノードのコンテンツ幅の最小値（SJISバイト数）
        /// </summary>
        static readonly int MinContentWidth = 50;

        /// <summary>
        /// QEDデータ構造をテキストデータに変換する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        static public string ToText(QuartetEditorDescription data)
        {
            StringBuilder export = new StringBuilder();
            int level = 0;
            foreach (var item in data.Node)
            {
                ToText(item, export, level);
            }
            return export.ToString();
        }

        /// <summary>
        /// ノードをテキストデータに変換する
        /// </summary>
        /// <returns></returns>
        static private void ToText(QuartetEditorDescriptionItem item, StringBuilder result, int level)
        {
            // タイトルを変換
            result.AppendLine(new string(' ', level * 2) + "【" + item.Name + "】");

            // コンテンツ変換
            string indent = new string(' ', (level + 1) * 2);
            int lineWidth = LinefeedWidth;
            if (LinefeedWidth - indent.GetSJISByte() < MinContentWidth)
            {
                lineWidth = indent.GetSJISByte() + MinContentWidth;
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
                ToText(child, result, level + 1);
            }
        }
        #endregion Text
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
