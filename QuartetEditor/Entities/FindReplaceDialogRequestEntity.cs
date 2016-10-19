using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    /// <summary>
    /// FindReplaceDialogへの要求
    /// </summary>
    public class FindReplaceDialogRequestEntity
    {
        /// <summary>
        /// 要求種別
        /// </summary>
        public DialogRequest RequestKind { set; get; }

        /// <summary>
        /// ダイアログへのリクエスト
        /// </summary>
        public enum DialogRequest
        {
            /// <summary>
            /// 「検索」する状態で開く
            /// </summary>
            OpenFind,

            /// <summary>
            /// 「置換」する状態で開く
            /// </summary>
            OpenReplace,

            /// <summary>
            /// 次を検索する
            /// </summary>
            FindNext,

            /// <summary>
            /// 前を検索する
            /// </summary>
            FindPrev,
        }
    }
}
