using QuartetEditor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    /// <summary>
    /// 検索結果
    /// </summary>
    public class SearchResult
    {
        /// <summary>
        /// 結果対象の種別
        /// </summary>
        public TargetType Type { set; get; }

        /// <summary>
        /// 開始位置
        /// </summary>
        public int Index { set; get; }

        /// <summary>
        /// 長さ
        /// </summary>
        public int Length { set; get; }

        /// <summary>
        /// ノード
        /// </summary>
        public Node Node { set; get; }

        /// <summary>
        /// 検索対象のタイプ
        /// </summary>
        public enum TargetType
        {
            /// <summary>
            /// テキストにヒット
            /// </summary>
            Content,

            /// <summary>
            /// タイトルにヒット
            /// </summary>
            Title,
        }
    }
}
