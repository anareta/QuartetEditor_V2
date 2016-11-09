using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    /// <summary>
    /// 検索置換条件
    /// </summary>
    public class FindReplaceEntity
    {
        /// <summary>
        /// 検索対象正規表現
        /// </summary>
        public Regex Find { get; set; }

        /// <summary>
        /// 置換文字列
        /// </summary>
        public string Replace { get; set; }

        /// <summary>
        /// 処理の種類
        /// </summary>
        public Action Kind { get; set; }

        /// <summary>
        /// 処理の種類
        /// </summary>
        public enum Action
        {
            /// <summary>
            /// 検索する
            /// </summary>
            Find,

            /// <summary>
            /// 置換する
            /// </summary>
            Replace,

            /// <summary>
            /// すべて置換する
            /// </summary>
            ReplaceAll,
        }
    }


}
