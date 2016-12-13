using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using QuartetEditor.Models;
using QuartetEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuartetEditor.Views.Controls
{
    /// <summary>
    /// ExportDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class ExportDialog : CustomDialog
    {
        /// <summary>
        /// エクスポートの実行通知
        /// </summary>
        public IObservable<ExportSettingModel> Excute => _Excute;

        private Subject<ExportSettingModel> _Excute = new Subject<ExportSettingModel>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ExportDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// ダイアログを閉じるときに実行
        /// </summary>
        protected override void OnClose()
        {
            var vm = this.DataContext as ExportDialogViewModel;
            if (vm?.CanExcute == true)
            {
                this._Excute.OnNext(vm?.Model);
            }
        }
    }
}
