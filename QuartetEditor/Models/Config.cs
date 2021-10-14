using Newtonsoft.Json;
using Prism.Mvvm;
using QuartetEditor.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor.Models
{
    /// <summary>
    /// 設定情報
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class Config : BindableBase
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Config()
        {
            this.OpenFilePath = "";
            this.TreeTextCharacters = "#";
            this.NodePanelFontFamily = new System.Windows.Media.FontFamily("メイリオ");

            this.CenterTextEditorFontSize = 14;
            this.CenterTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.CenterTextEditorLineHeight = 5;
            this.CenterTextEditorTextWrapping = true;
            this.HighlightCurrentLine = false;
            this.ShowLineNumbers = false;
            this.HeaderCharacters = "◇◆";
            this.RestoreCenterScrolledLine = true;

            this.NodePanelWidth = null;
            this.MainPanelWidth = null;
        }

        /// <summary>
        /// 階層付きテキストの見出し文字
        /// </summary>
        [JsonProperty]
        public string TreeTextCharacters
        {
            get { return this._TreeTextCharacters; }
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    this.SetProperty(ref this._TreeTextCharacters, value);
                }
            }
        }

        public string _TreeTextCharacters;

        /// <summary>
        /// ノードパネルのフォント
        /// </summary>
        [JsonProperty]
        public System.Windows.Media.FontFamily NodePanelFontFamily
        {
            get { return this._NodePanelFontFamily; }
            set { this.SetProperty(ref this._NodePanelFontFamily, value); }
        }

        public System.Windows.Media.FontFamily _NodePanelFontFamily;


        [JsonProperty]
        public string OpenFilePath
        {
            get { return this._OpenFilePath; }
            set { this.SetProperty(ref this._OpenFilePath, value); }
        }
        public string _OpenFilePath;

        #region CenterTextEditor
        /// <summary>
        /// 編集パネルのフォントサイズ
        /// </summary>
        [JsonProperty]
        public double CenterTextEditorFontSize
        {
            get { return this._CenterTextEditorFontSize; }
            set { this.SetProperty(ref this._CenterTextEditorFontSize, value); }
        }
        public double _CenterTextEditorFontSize;

        /// <summary>
        /// 編集パネルのフォント
        /// </summary>
        [JsonProperty]
        public System.Windows.Media.FontFamily CenterTextEditorFontFamily
        {
            get { return this._CenterTextEditorFontFamily; }
            set { this.SetProperty(ref this._CenterTextEditorFontFamily, value); }
        }
        public System.Windows.Media.FontFamily _CenterTextEditorFontFamily;

        /// <summary>
        /// 編集パネルの行間
        /// </summary>
        [JsonProperty]
        public double CenterTextEditorLineHeight
        {
            get { return this._CenterTextEditorLineHeight; }
            set { this.SetProperty(ref this._CenterTextEditorLineHeight, value); }
        }
        public double _CenterTextEditorLineHeight;

        /// <summary>
        /// 編集パネルの折り返し
        /// </summary>
        [JsonProperty]
        public bool CenterTextEditorTextWrapping
        {
            get { return this._CenterTextEditorTextWrapping; }
            set { this.SetProperty(ref this._CenterTextEditorTextWrapping, value); }
        }
        public bool _CenterTextEditorTextWrapping;

        /// <summary>
        /// 編集パネルで現在行のハイライト表示
        /// </summary>
        [JsonProperty]
        public bool HighlightCurrentLine
        {
            get { return this._HighlightCurrentLine; }
            set { this.SetProperty(ref this._HighlightCurrentLine, value); }
        }
        public bool _HighlightCurrentLine;

        /// <summary>
        /// 編集パネルで行番号の表示
        /// </summary>
        [JsonProperty]
        public bool ShowLineNumbers
        {
            get { return this._ShowLineNumbers; }
            set { this.SetProperty(ref this._ShowLineNumbers, value); }
        }
        public bool _ShowLineNumbers;

        /// <summary>
        /// 編集パネルでヘッダー文字列
        /// </summary>
        [JsonProperty]
        public string HeaderCharacters
        {
            get { return this._HeaderCharacters; }
            set { this.SetProperty(ref this._HeaderCharacters, value); }
        }
        public string _HeaderCharacters;

        /// <summary>
        /// 編集パネルで最後にスクロールした位置を復元する
        /// </summary>
        [JsonProperty]
        public bool RestoreCenterScrolledLine
        {
            get { return this._RestoreCenterScrolledLine; }
            set { this.SetProperty(ref this._RestoreCenterScrolledLine, value); }
        }
        public bool _RestoreCenterScrolledLine;

        #endregion CenterTextEditor

        #region PanelSize

        /// <summary>
        /// ノードパネルの幅
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double? NodePanelWidth
        {
            get { return this._NodePanelWidth; }
            set { this.SetProperty(ref this._NodePanelWidth, value); }
        }
        public double? _NodePanelWidth;

        /// <summary>
        /// メインパネルの幅
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double? MainPanelWidth
        {
            get { return this._MainPanelWidth; }
            set { this.SetProperty(ref this._MainPanelWidth, value); }
        }
        public double? _MainPanelWidth;

        #endregion PanelSize

    }
}
