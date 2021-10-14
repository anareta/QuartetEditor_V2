using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor.Models
{
    /// <summary>
    /// エクスポートの設定クラス
    /// </summary>
    public class ExportSettingModel : BindableBase
    {
        /// <summary>
        /// エクスポート種別
        /// </summary>
        public ObservableCollection<Tuple<ExportKind, string>> ComboBoxItemSource { get; } = new ObservableCollection<Tuple<ExportKind, string>>();

        /// <summary>
        /// エクスポートの種別
        /// </summary>
        private ExportKind _Kind;

        public ExportKind Kind
        {
            get
            {
                return this._Kind;
            }
            set
            {
                this.SetProperty(ref this._Kind, value);
            }
        }

        #region Text
        /// <summary>
        /// テキストを折り返すか
        /// </summary>
        private bool _EnableLineWrap = true;

        public bool EnableLineWrap
        {
            get
            {
                return this._EnableLineWrap;
            }
            set
            {
                this.SetProperty(ref this._EnableLineWrap, value);
            }
        }

        /// <summary>
        /// テキストの折り返し幅
        /// </summary>
        private int _LineWrap = 90;

        public int LineWrap
        {
            get
            {
                return this._LineWrap;
            }
            set
            {
                this.SetProperty(ref this._LineWrap, value);
            }
        }
        #endregion Text

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public ExportSettingModel()
        {
            this.ComboBoxItemSource.Add(new Tuple<ExportKind, string>(ExportKind.Text, "テキストファイル"));
            this.ComboBoxItemSource.Add(new Tuple<ExportKind, string>(ExportKind.HTML, "HTMLファイル"));

        }

    }
}
