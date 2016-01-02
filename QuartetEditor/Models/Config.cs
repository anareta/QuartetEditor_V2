using Newtonsoft.Json;
using Prism.Mvvm;
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
            this.CenterTextEditorFontSize = 14;
            this.CenterTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.CenterTextEditorLineHeight = 5;
            this.CenterTextEditorTextWrapping = true;
            this.HighlightCurrentLine = false;
            this.ShowLineNumbers = false;
            this.HeaderCharactors = "□■◇◆";

            this.LeftTextEditorFontSize = 10;
            this.LeftTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.LeftTextEditorLineHeight = 1;
            this.LeftTextEditorTextWrapping = false;

            this.TopBottomTextEditorFontSize = 12;
            this.TopBottomTextEditorFontFamily = new System.Windows.Media.FontFamily("メイリオ");
            this.TopBottomTextEditorLineHeight = 1;
            this.TopBottomTextEditorTextWrapping = false;

            this.LeftPanelOpen = true;
            this.TopPanelOpen = true;
            this.BottomPanelOpen = true;
        }

        #region CenterTextEditor
        /// <summary>
        /// 編集パネルのフォントサイズ
        /// </summary>
        public double _CenterTextEditorFontSize;

        [JsonProperty]
        public double CenterTextEditorFontSize
        {
            get { return this._CenterTextEditorFontSize; }
            set { this.SetProperty(ref this._CenterTextEditorFontSize, value); }
        }

        /// <summary>
        /// 編集パネルのフォント
        /// </summary>
        public System.Windows.Media.FontFamily _CenterTextEditorFontFamily;

        [JsonProperty]
        public System.Windows.Media.FontFamily CenterTextEditorFontFamily
        {
            get { return this._CenterTextEditorFontFamily; }
            set { this.SetProperty(ref this._CenterTextEditorFontFamily, value); }
        }


        /// <summary>
        /// 編集パネルの行間
        /// </summary>
        public double _CenterTextEditorLineHeight;

        [JsonProperty]
        public double CenterTextEditorLineHeight
        {
            get { return this._CenterTextEditorLineHeight; }
            set { this.SetProperty(ref this._CenterTextEditorLineHeight, value); }
        }

        /// <summary>
        /// 編集パネルの折り返し
        /// </summary>
        public bool _CenterTextEditorTextWrapping;

        [JsonProperty]
        public bool CenterTextEditorTextWrapping
        {
            get { return this._CenterTextEditorTextWrapping; }
            set { this.SetProperty(ref this._CenterTextEditorTextWrapping, value); }
        }

        /// <summary>
        /// 編集パネルで現在行のハイライト表示
        /// </summary>
        public bool _HighlightCurrentLine;

        [JsonProperty]
        public bool HighlightCurrentLine
        {
            get { return this._HighlightCurrentLine; }
            set { this.SetProperty(ref this._HighlightCurrentLine, value); }
        }

        /// <summary>
        /// 編集パネルで行番号の表示
        /// </summary>
        public bool _ShowLineNumbers;

        [JsonProperty]
        public bool ShowLineNumbers
        {
            get { return this._ShowLineNumbers; }
            set { this.SetProperty(ref this._ShowLineNumbers, value); }
        }

        /// <summary>
        /// 編集パネルでヘッダー文字列
        /// </summary>
        public string _HeaderCharactors;

        [JsonProperty]
        public string HeaderCharactors
        {
            get { return this._HeaderCharactors; }
            set { this.SetProperty(ref this._HeaderCharactors, value); }
        }

        #endregion CenterTextEditor



        #region LeftTextEditor
        /// <summary>
        /// 左参照パネルフォントサイズ
        /// </summary>
        public double _LeftTextEditorFontSize;

        [JsonProperty]
        public double LeftTextEditorFontSize
        {
            get { return this._LeftTextEditorFontSize; }
            set { this.SetProperty(ref this._LeftTextEditorFontSize, value); }
        }

        /// <summary>
        /// 左参照パネルフォント
        /// </summary>
        public System.Windows.Media.FontFamily _LeftTextEditorFontFamily;

        [JsonProperty]
        public System.Windows.Media.FontFamily LeftTextEditorFontFamily
        {
            get { return this._LeftTextEditorFontFamily; }
            set { this.SetProperty(ref this._LeftTextEditorFontFamily, value); }
        }

        /// <summary>
        /// 左参照パネルの行間
        /// </summary>
        public double _LeftTextEditorLineHeight;

        [JsonProperty]
        public double LeftTextEditorLineHeight
        {
            get { return this._LeftTextEditorLineHeight; }
            set { this.SetProperty(ref this._LeftTextEditorLineHeight, value); }
        }

        /// <summary>
        /// 左参照パネルの折り返し
        /// </summary>
        public bool _LeftTextEditorTextWrapping;

        [JsonProperty]
        public bool LeftTextEditorTextWrapping
        {
            get { return this._LeftTextEditorTextWrapping; }
            set { this.SetProperty(ref this._LeftTextEditorTextWrapping, value); }
        }

        #endregion LeftTextEditor



        #region TopBottomTextEditor
        /// <summary>
        /// 上下参照パネルのフォントサイズ
        /// </summary>
        public double _TopBottomTextEditorFontSize;

        [JsonProperty]
        public double TopBottomTextEditorFontSize
        {
            get { return this._TopBottomTextEditorFontSize; }
            set { this.SetProperty(ref this._TopBottomTextEditorFontSize, value); }
        }

        /// <summary>
        /// 上下参照パネルのフォント
        /// </summary>
        public System.Windows.Media.FontFamily _TopBottomTextEditorFontFamily;

        [JsonProperty]
        public System.Windows.Media.FontFamily TopBottomTextEditorFontFamily
        {
            get { return this._TopBottomTextEditorFontFamily; }
            set { this.SetProperty(ref this._TopBottomTextEditorFontFamily, value); }
        }

        /// <summary>
        /// 上下参照パネルの行間
        /// </summary>
        public double _TopBottomTextEditorLineHeight;

        [JsonProperty]
        public double TopBottomTextEditorLineHeight
        {
            get { return this._TopBottomTextEditorLineHeight; }
            set { this.SetProperty(ref this._TopBottomTextEditorLineHeight, value); }
        }

        /// <summary>
        /// 上下参照パネルの折り返し
        /// </summary>
        public bool _TopBottomTextEditorTextWrapping;

        [JsonProperty]
        public bool TopBottomTextEditorTextWrapping
        {
            get { return this._TopBottomTextEditorTextWrapping; }
            set { this.SetProperty(ref this._TopBottomTextEditorTextWrapping, value); }
        }

        #endregion TopBottomTextEditor


        #region PanelState

        /// <summary>
        /// 左パネルの開閉状態
        /// </summary>
        public bool _LeftPanelOpen;

        [JsonProperty]
        public bool LeftPanelOpen
        {
            get { return this._LeftPanelOpen; }
            set { this.SetProperty(ref this._LeftPanelOpen, value); }
        }

        /// <summary>
        /// 上パネルの開閉状態
        /// </summary>
        public bool _TopPanelOpen;

        [JsonProperty]
        public bool TopPanelOpen
        {
            get { return this._TopPanelOpen; }
            set { this.SetProperty(ref this._TopPanelOpen, value); }
        }

        /// <summary>
        /// 下パネルの開閉状態
        /// </summary>
        public bool _BottomPanelOpen;

        [JsonProperty]
        public bool BottomPanelOpen
        {
            get { return this._BottomPanelOpen; }
            set { this.SetProperty(ref this._BottomPanelOpen, value); }
        }

        #endregion PanelState

    }
}
