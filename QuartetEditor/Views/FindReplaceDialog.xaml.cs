using System.Windows;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using System.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using QuartetEditor.ViewModels;
using QuartetEditor.Models;

namespace QuartetEditor.Views
{
    /// <summary>
    /// FindReplaceDialog
    /// </summary>
    public partial class FindReplaceDialog : MetroWindow
    {
        /// <summary>
        /// ダイアログ
        /// </summary>
        private static FindReplaceDialog _Dialog = null;

        /// <summary>
        /// 検索条件
        /// </summary>
        private static Search _Model { get; } = new Search();

        /// <summary>
        /// テキストエディタへの参照
        /// </summary>
        public TextEditor Editor { get; }

        /// <summary>
        /// FindReplaceDialog
        /// </summary>
        /// <param name="editor"></param>
        public FindReplaceDialog(TextEditor editor)
        {
            this.DataContext = new FindReplaceDialogViewModel(_Model, editor);
            this.Editor = editor;

            InitializeComponent();
        }

        /// <summary>
        /// ハイライトの表示状態を更新
        /// </summary>
        public void UpdateHighlight()
        {
            ((FindReplaceDialogViewModel)_Dialog.DataContext).UpdateHighlight();
        }

        /// <summary>
        /// ウィンドウを閉じる
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, System.EventArgs e)
        {
            ((FindReplaceDialogViewModel)this.DataContext).Dispose();
            _Dialog = null;
        }

        /// <summary>
        /// 検索を実行
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="editor"></param>
        /// <param name="prev"></param>
        public static void Find(Window owner, TextEditor editor, bool prev)
        {
            if (_Dialog != null)
            {
                if (!prev)
                {
                    ((FindReplaceDialogViewModel)_Dialog.DataContext).FindNextCommand.Execute();
                }
                else
                {
                    ((FindReplaceDialogViewModel)_Dialog.DataContext).FindPrevCommand.Execute();
                }
            }
        }

        /// <summary>
        /// ダイアログを表示
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="editor"></param>
        /// <param name="isFind"></param>
        public static void ShowFindReplaceDialog(Window owner, TextEditor editor, bool isFind = true)
        {
            if (_Dialog == null)
            {
                _OpenDialog(owner, editor, isFind);
            }
            else
            {
                if (isFind)
                {
                    _Dialog.findTab.IsSelected = true;
                    
                }
                else
                {
                    _Dialog.replaceTab.IsSelected = true;
                }
            }
            _Dialog.Activate();

            if (!string.IsNullOrEmpty(editor.TextArea.Selection.GetText()) && 
                !editor.TextArea.Selection.IsMultiline)
            {
                _Dialog.txtFind.Text = editor.TextArea.Selection.GetText();
                _Dialog.txtFind2.Text = editor.TextArea.Selection.GetText();
            }

            if (isFind)
            {
                _Dialog.txtFind.SelectAll();
                _Dialog.txtFind.Focus();
            }
            else
            {
                _Dialog.txtFind2.SelectAll();
                _Dialog.txtFind2.Focus();
            }
        }

        /// <summary>
        /// ダイアログを生成する
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="editor"></param>
        /// <param name="isFind"></param>
        private static void _OpenDialog(Window owner, TextEditor editor, bool isFind)
        {
            _Dialog = new FindReplaceDialog(editor);

            if (isFind)
            {
                _Dialog.findTab.IsSelected = true;
            }
            else
            {
                _Dialog.replaceTab.IsSelected = true;
            }

            _Dialog.Owner = owner;
            _Dialog.Show();
        }
    }
}
