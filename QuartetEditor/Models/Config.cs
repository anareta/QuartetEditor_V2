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
            this.TreeTextCharacters = ".";
            this.NodePanelFontFamily = new System.Windows.Media.FontFamily("メイリオ");

            this.CenterTextEditorFontSize = 14;
            this.CenterTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.CenterTextEditorLineHeight = 5;
            this.CenterTextEditorTextWrapping = true;
            this.HighlightCurrentLine = false;
            this.ShowLineNumbers = false;
            this.HeaderCharacters = "□■◇◆";
            this.RestoreCenterScrolledLine = false;

            this.LeftTextEditorFontSize = 10;
            this.LeftTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.LeftTextEditorLineHeight = 1;
            this.LeftTextEditorTextWrapping = false;
            this.RestoreLeftScrolledLine = false;

            this.TopBottomTextEditorFontSize = 12;
            this.TopBottomTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.TopBottomTextEditorLineHeight = 1;
            this.TopBottomTextEditorTextWrapping = false;
            this.RestoreTopBottomScrolledLine = false;

            this.LeftPanelOpen = true;
            this.TopPanelOpen = true;
            this.BottomPanelOpen = true;

            this.LeftPanelWidth = 1;
            this.TopPanelHeight = 1;
            this.BottomPanelHeight = 1;
            this.NodePanelWidth = 1;
            this.MainPanelHeight = 5;
            this.MainPanelWidth = 3;
        }

        #region General

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

        #endregion General


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
        /// 編集パネルで改行の表示
        /// </summary>
        [JsonProperty]
        public bool ShowEndOfLine
        {
            get { return this._ShowEndOfLine; }
            set { this.SetProperty(ref this._ShowEndOfLine, value); }
        }
        public bool _ShowEndOfLine;

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



        #region LeftTextEditor
        /// <summary>
        /// 左参照パネルフォントサイズ
        /// </summary>
        [JsonProperty]
        public double LeftTextEditorFontSize
        {
            get { return this._LeftTextEditorFontSize; }
            set { this.SetProperty(ref this._LeftTextEditorFontSize, value); }
        }
        public double _LeftTextEditorFontSize;

        /// <summary>
        /// 左参照パネルフォント
        /// </summary>
        [JsonProperty]
        public System.Windows.Media.FontFamily LeftTextEditorFontFamily
        {
            get { return this._LeftTextEditorFontFamily; }
            set { this.SetProperty(ref this._LeftTextEditorFontFamily, value); }
        }
        public System.Windows.Media.FontFamily _LeftTextEditorFontFamily;

        /// <summary>
        /// 左参照パネルの行間
        /// </summary>
        [JsonProperty]
        public double LeftTextEditorLineHeight
        {
            get { return this._LeftTextEditorLineHeight; }
            set { this.SetProperty(ref this._LeftTextEditorLineHeight, value); }
        }
        public double _LeftTextEditorLineHeight;

        /// <summary>
        /// 左参照パネルの折り返し
        /// </summary>
        [JsonProperty]
        public bool LeftTextEditorTextWrapping
        {
            get { return this._LeftTextEditorTextWrapping; }
            set { this.SetProperty(ref this._LeftTextEditorTextWrapping, value); }
        }
        public bool _LeftTextEditorTextWrapping;

        /// <summary>
        /// 左参照パネルで最後にスクロールした位置を復元する
        /// </summary>
        [JsonProperty]
        public bool RestoreLeftScrolledLine
        {
            get { return this._RestoreLeftScrolledLine; }
            set { this.SetProperty(ref this._RestoreLeftScrolledLine, value); }
        }
        public bool _RestoreLeftScrolledLine;

        #endregion LeftTextEditor



        #region TopBottomTextEditor
        /// <summary>
        /// 上下参照パネルのフォントサイズ
        /// </summary>
        [JsonProperty]
        public double TopBottomTextEditorFontSize
        {
            get { return this._TopBottomTextEditorFontSize; }
            set { this.SetProperty(ref this._TopBottomTextEditorFontSize, value); }
        }
        public double _TopBottomTextEditorFontSize;

        /// <summary>
        /// 上下参照パネルのフォント
        /// </summary>
        [JsonProperty]
        public System.Windows.Media.FontFamily TopBottomTextEditorFontFamily
        {
            get { return this._TopBottomTextEditorFontFamily; }
            set { this.SetProperty(ref this._TopBottomTextEditorFontFamily, value); }
        }
        public System.Windows.Media.FontFamily _TopBottomTextEditorFontFamily;

        /// <summary>
        /// 上下参照パネルの行間
        /// </summary>
        [JsonProperty]
        public double TopBottomTextEditorLineHeight
        {
            get { return this._TopBottomTextEditorLineHeight; }
            set { this.SetProperty(ref this._TopBottomTextEditorLineHeight, value); }
        }
        public double _TopBottomTextEditorLineHeight;

        /// <summary>
        /// 上下参照パネルの折り返し
        /// </summary>
        [JsonProperty]
        public bool TopBottomTextEditorTextWrapping
        {
            get { return this._TopBottomTextEditorTextWrapping; }
            set { this.SetProperty(ref this._TopBottomTextEditorTextWrapping, value); }
        }
        public bool _TopBottomTextEditorTextWrapping;

        /// <summary>
        /// 上下参照パネルで最後にスクロールした位置を復元する
        /// </summary>
        [JsonProperty]
        public bool RestoreTopBottomScrolledLine
        {
            get { return this._RestoreTopBottomScrolledLine; }
            set { this.SetProperty(ref this._RestoreTopBottomScrolledLine, value); }
        }
        public bool _RestoreTopBottomScrolledLine;

        #endregion TopBottomTextEditor


        #region PanelState

        /// <summary>
        /// 左パネルの開閉状態
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public bool LeftPanelOpen
        {
            get { return this._LeftPanelOpen; }
            set { this.SetProperty(ref this._LeftPanelOpen, value); }
        }
        public bool _LeftPanelOpen;

        /// <summary>
        /// 上パネルの開閉状態
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public bool TopPanelOpen
        {
            get { return this._TopPanelOpen; }
            set { this.SetProperty(ref this._TopPanelOpen, value); }
        }
        public bool _TopPanelOpen;

        /// <summary>
        /// 下パネルの開閉状態
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public bool BottomPanelOpen
        {
            get { return this._BottomPanelOpen; }
            set { this.SetProperty(ref this._BottomPanelOpen, value); }
        }
        public bool _BottomPanelOpen;

        #endregion PanelState

        #region PanelSize

        /// <summary>
        /// 左パネルの幅
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double LeftPanelWidth
        {
            get { return this._LeftPanelWidth; }
            set { this.SetProperty(ref this._LeftPanelWidth, value); }
        }
        public double _LeftPanelWidth;

        /// <summary>
        /// 上パネルの高さ
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double TopPanelHeight
        {
            get { return this._TopPanelHeight; }
            set { this.SetProperty(ref this._TopPanelHeight, value); }
        }
        public double _TopPanelHeight;

        /// <summary>
        /// 下パネルの高さ
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double BottomPanelHeight
        {
            get { return this._BottomPanelHeight; }
            set { this.SetProperty(ref this._BottomPanelHeight, value); }
        }
        public double _BottomPanelHeight;

        /// <summary>
        /// ノードパネルの幅
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double NodePanelWidth
        {
            get { return this._NodePanelWidth; }
            set { this.SetProperty(ref this._NodePanelWidth, value); }
        }
        public double _NodePanelWidth;

        /// <summary>
        /// メインパネルの高さ
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double MainPanelHeight
        {
            get { return this._MainPanelHeight; }
            set { this.SetProperty(ref this._MainPanelHeight, value); }
        }
        public double _MainPanelHeight;

        /// <summary>
        /// メインパネルの幅
        /// </summary>
        [JsonProperty]
        [Transaction(IsEnlist = false)]
        public double MainPanelWidth
        {
            get { return this._MainPanelWidth; }
            set { this.SetProperty(ref this._MainPanelWidth, value); }
        }
        public double _MainPanelWidth;

        #endregion PanelSize

    }
}
