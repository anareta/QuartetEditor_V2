using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.IO;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Xml;

namespace QuartetEditor.Views.Controls
{
    /// <summary>
    /// バインド可能なTextEditorクラス
    /// TextEditorクラスのラッパー
    /// </summary>
    public class BindableTextEditor : TextEditor
    {
        /// <summary>
        /// バインド可能なTextプロパティ
        /// </summary>
        public static readonly DependencyProperty BindableTextProperty = DependencyProperty.Register(
            "BindableText", typeof(string), typeof(BindableTextEditor), new PropertyMetadata("", OnBindableTextChanged));

        /// <summary>
        /// テキスト変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnBindableTextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (BindableTextEditor)sender;
            if (string.Compare(control.BindableText, e.NewValue?.ToString()) != 0)
            {
                //avoid undo stack overflow
                control.BindableText = e.NewValue?.ToString();
            }
        }

        /// <summary>
        /// バインド可能なText
        /// </summary>
        public string BindableText
        {
            get
            {
                return this.Text;
            }
            set
            {
                this.Text = value;
            }
        }

        /// <summary>
        /// 基底クラスのテキストが変更した場合の処理をオーバーライド
        /// バインド先にも通知する
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            SetCurrentValue(BindableTextProperty, this.Text);
            base.OnTextChanged(e);
        }


        /// <summary>
        /// 選択行のハイライト設定
        /// </summary>
        public static readonly DependencyProperty HighlightCurrentLineProperty = DependencyProperty.Register(
            "HighlightCurrentLine", typeof(bool), typeof(BindableTextEditor), new PropertyMetadata(false, OnHighlightCurrentLineChanged));


        /// <summary>
        /// 選択行のハイライト設定
        /// </summary>
        public bool HighlightCurrentLine
        {
            get
            {
                if (this.Options == null)
                {
                    this.Options = new TextEditorOptions();
                }
                return this.Options.HighlightCurrentLine;
            }
            set
            {
                if (this.Options == null)
                {
                    this.Options = new TextEditorOptions();
                }
                this.Options.HighlightCurrentLine = value;
            }
        }

        /// <summary>
        /// 選択行のハイライト設定が変更された場合
        /// </summary>
        private static void OnHighlightCurrentLineChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (BindableTextEditor)sender;
            control.HighlightCurrentLine = (bool)e.NewValue;
        }


        /// <summary>
        /// 選択行のハイライト設定
        /// </summary>
        public static readonly DependencyProperty ShowEndOfLineProperty = DependencyProperty.Register(
            nameof(ShowEndOfLine), typeof(bool), typeof(BindableTextEditor), new PropertyMetadata(false, OnShowEndOfLineChanged));

        /// <summary>
        /// 改行文字の表示
        /// </summary>
        public bool ShowEndOfLine
        {
            get
            {
                if (this.Options == null)
                {
                    this.Options = new TextEditorOptions();
                }
                return this.Options.ShowEndOfLine;
            }
            set
            {
                if (this.Options == null)
                {
                    this.Options = new TextEditorOptions();
                }
                this.Options.ShowEndOfLine = value;
            }
        }

        /// <summary>
        /// 改行文字の表示が変更された場合
        /// </summary>
        private static void OnShowEndOfLineChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (BindableTextEditor)sender;
            control.ShowEndOfLine = (bool)e.NewValue;
        }

        /// <summary>
        /// 見出し行の文字
        /// </summary>
        public string HeaderCharacters
        {
            get
            {
                return (string)GetValue(HeaderCharactersProperty);
            }
            set
            {
                SetValue(HeaderCharactersProperty, value);
                this.UpdateHightlightSetting();
            }
        }

        /// <summary>
        /// 見出し行の文字
        /// </summary>
        public static readonly DependencyProperty HeaderCharactersProperty =
            DependencyProperty.Register("HeaderCharacters", typeof(string), typeof(BindableTextEditor), new PropertyMetadata("", OnHeaderCharactersChanged));

        /// <summary>
        /// 見出し行の文字の変更時処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnHeaderCharactersChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = (BindableTextEditor)sender;
            control.HeaderCharacters = (string)e.NewValue;
            control.UpdateHightlightSetting();
        }

        /// <summary>
        /// ハイライトの設定変更
        /// </summary>
        private void UpdateHightlightSetting()
        {
            if (string.IsNullOrWhiteSpace(this.HeaderCharacters))
            {
                return;
            }

            try
            {
                using (Stream s = typeof(MainWindow).Assembly.GetManifestResourceStream("QuartetEditor.HighlightingRules.xshd"))
                {
                    if (s == null)
                    {
                        throw new InvalidOperationException();
                    }

                    using (StreamReader resourceReader = new StreamReader(s))
                    {
                        string headers = default(string);
                        foreach (var c in this.HeaderCharacters.Trim())
                        {
                            headers += "|" + Regex.Escape(new string(new char[1] { c }));
                        }
                        headers = headers.Remove(0, 1);

                        using (XmlReader reader = new XmlTextReader(new MemoryStream(Encoding.Unicode.GetBytes(resourceReader.ReadToEnd().Replace("@", headers)))))
                        {
                            base.SyntaxHighlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        /// <summary>
        /// テキストボックスが選択されているか
        /// </summary>
        public bool IsSelected { get; private set; }

        /// <summary>
        /// バインド可能なTextEditorクラス
        /// TextEditorクラスのラッパー
        /// </summary>
        public BindableTextEditor()
        {
            Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
            h => (s, e) => h(e),
            h => this.GotFocus += h,
            h => this.GotFocus -= h)
            .Subscribe(_ => this.IsSelected = true);

            Observable.FromEvent<RoutedEventHandler, RoutedEventArgs>(
            h => (s, e) => h(e),
            h => this.LostFocus += h,
            h => this.LostFocus -= h)
            .Subscribe(_ => this.IsSelected = false);

            this.Options = new TextEditorOptions();
        }
    }
}
