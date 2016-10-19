using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Entities
{
    /// <summary>
    /// ダイアログを開くための変数
    /// </summary>
    class DialogArg
    {
        /// <summary>
        /// ダイアログのタイトル
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// ダイアログに表示するメッセージ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ダイアログの形
        /// </summary>
        public MessageDialogStyle Style { get; set; }

        /// <summary>
        /// ダイアログの設定
        /// </summary>
        public MetroDialogSettings Settings { get; set; }

        /// <summary>
        /// ダイアログの結果
        /// </summary>
        public MessageDialogResult Result { get; set; }
    }
}
