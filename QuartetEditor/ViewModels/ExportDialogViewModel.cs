using Prism.Mvvm;
using QuartetEditor.Enums;
using QuartetEditor.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Reactive.Linq;

namespace QuartetEditor.ViewModels
{
    /// <summary>
    /// エクスポートダイアログ
    /// </summary>
    class ExportDialogViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// モデル
        /// </summary>
        public ExportSettingModel Model { get; }

        /// <summary>
        /// エクスポートの実行可否
        /// </summary>
        public bool CanExcute { set; get; } = false;

        /// <summary>
        /// ダイアログを閉じる
        /// </summary>
        public ReactiveCommand CloseCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// エクスポートを実行
        /// </summary>
        public ReactiveCommand ExcuteCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// エクスポート種別
        /// </summary>
        public ReadOnlyReactiveCollection<Tuple<ExportKind, string>> ComboBoxItemSource { get; }

        /// <summary>
        /// 選択されたエクスポート種別
        /// </summary>
        public ReactiveProperty<ExportKind> SelectedValue { get; }

        #region Text

        /// <summary>
        /// テキストを折り返すか否か
        /// </summary>
        public ReactiveProperty<bool> EnableLineWrap { get; }

        /// <summary>
        /// テキストの折り返し幅（バイト）
        /// </summary>
        public ReactiveProperty<int> LineWrap { get; }

        /// <summary>
        /// テキスト用の詳細設定の表示
        /// </summary>
        public ReactiveProperty<Visibility> TextSettingVisibility { get; }

        #endregion Text

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        /// <param name="closeAction">ダイアログを閉じるクロージャ</param>
        public ExportDialogViewModel(Action closeAction)
        {
            this.CloseCommand.Subscribe(_ =>
            {
                this.CanExcute = false;
                closeAction();
            }).AddTo(this.Disposable);

            this.ExcuteCommand.Subscribe(_ =>
            {
                this.CanExcute = true;
                closeAction();
            });

            this.Model = new ExportSettingModel();

            this.ComboBoxItemSource = this.Model
                .ComboBoxItemSource
                .ToReadOnlyReactiveCollection();

            this.EnableLineWrap = this.Model.ToReactivePropertyAsSynchronized(x => x.EnableLineWrap)
                                      .AddTo(this.Disposable);

            this.LineWrap = this.Model.ToReactivePropertyAsSynchronized(x => x.LineWrap)
                                .AddTo(this.Disposable);

            this.SelectedValue = this.Model.ToReactivePropertyAsSynchronized(x => x.Kind)
                                     .AddTo(this.Disposable);

            this.TextSettingVisibility = this.SelectedValue
                                 .Select(x => x == ExportKind.Text ? Visibility.Visible : Visibility.Hidden)
                                 .ToReactiveProperty()
                                 .AddTo(this.Disposable);

        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            Disposable.Dispose();
        }
    }
}
