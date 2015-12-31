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
    public class Config
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
        public double CenterTextEditorFontSize { set; get; }

        /// <summary>
        /// 編集パネルのフォント
        /// </summary>
        public System.Windows.Media.FontFamily CenterTextEditorFontFamily { set; get; }

        /// <summary>
        /// 編集パネルの行間
        /// </summary>
        public double CenterTextEditorLineHeight { set; get; }

        /// <summary>
        /// 編集パネルの折り返し
        /// </summary>
        public bool CenterTextEditorTextWrapping { set; get; }

        /// <summary>
        /// 編集パネルで現在行のハイライト表示
        /// </summary>
        public bool HighlightCurrentLine { set; get; }

        /// <summary>
        /// 編集パネルで行番号の表示
        /// </summary>
        public bool ShowLineNumbers { set; get; }

        /// <summary>
        /// 編集パネルでヘッダー文字列
        /// </summary>
        public string HeaderCharactors { set; get; }

        #endregion CenterTextEditor



        #region LeftTextEditor
        /// <summary>
        /// 左参照パネルフォントサイズ
        /// </summary>
        public double LeftTextEditorFontSize { set; get; }

        /// <summary>
        /// 左参照パネルフォント
        /// </summary>
        public System.Windows.Media.FontFamily LeftTextEditorFontFamily { set; get; }

        /// <summary>
        /// 左参照パネルの行間
        /// </summary>
        public double LeftTextEditorLineHeight { set; get; }

        /// <summary>
        /// 左参照パネルの折り返し
        /// </summary>
        public bool LeftTextEditorTextWrapping { set; get; }

        #endregion LeftTextEditor



        #region TopBottomTextEditor
        /// <summary>
        /// 上下参照パネルのフォントサイズ
        /// </summary>
        public double TopBottomTextEditorFontSize { set; get; }

        /// <summary>
        /// 上下参照パネルのフォント
        /// </summary>
        public System.Windows.Media.FontFamily TopBottomTextEditorFontFamily { set; get; }

        /// <summary>
        /// 上下参照パネルの行間
        /// </summary>
        public double TopBottomTextEditorLineHeight { set; get; }

        /// <summary>
        /// 上下参照パネルの折り返し
        /// </summary>
        public bool TopBottomTextEditorTextWrapping { set; get; }

        #endregion TopBottomTextEditor


        #region PanelState

        /// <summary>
        /// 左パネルの開閉状態
        /// </summary>
        public bool LeftPanelOpen { set; get; }

        /// <summary>
        /// 上パネルの開閉状態
        /// </summary>
        public bool TopPanelOpen { set; get; }

        /// <summary>
        /// 下パネルの開閉状態
        /// </summary>
        public bool BottomPanelOpen { set; get; }

        #endregion PanelState
    }
}
