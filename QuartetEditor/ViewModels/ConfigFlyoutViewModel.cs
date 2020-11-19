using Prism.Mvvm;
using QuartetEditor.Attributes;
using QuartetEditor.Models;
using Reactive.Bindings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Reactive.Bindings.Extensions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using QuartetEditor.ItemSources;

namespace QuartetEditor.ViewModels
{
    class ConfigFlyoutViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// モデルクラス
        /// </summary>
        public Config Model { get; }

        /// <summary>
        /// 変更情報
        /// </summary>
        Dictionary<string, object> _backings;

        /// <summary>
        /// フォント一覧に表示するフォントのリスト
        /// </summary>
        public List<System.Windows.Media.FontFamily> FontList
        {
            get
            {
                return FontFamilyItemSource.Get();
            }
        }

        /// <summary>
        /// 階層付きテキストの制御文字のリスト
        /// </summary>
        public List<KeyValuePair<string, string>> TreeTextCharactersList
        {
            get
            {
                return new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(".（ドット）", "."),
                    new KeyValuePair<string, string>("#（シャープ）", "#")
                };
            }
        }

        #region General

        /// <summary>
        /// 階層付きテキストの見出し文字
        /// </summary>
        public ReactiveProperty<string> TreeTextCharacters { get; }

        /// <summary>
        /// ノードパネルのフォント
        /// </summary>
        public ReactiveProperty<System.Windows.Media.FontFamily> NodePanelFontFamily { get; }

        #endregion General

        #region CenterTextEditor
        /// <summary>
        /// 編集パネルのフォントサイズ
        /// </summary>
        public ReactiveProperty<double> CenterTextEditorFontSize { get; }

        /// <summary>
        /// 編集パネルのフォント
        /// </summary>
        public ReactiveProperty<System.Windows.Media.FontFamily> CenterTextEditorFontFamily { get; }

        /// <summary>
        /// 編集パネルの行間
        /// </summary>
        public ReactiveProperty<double> CenterTextEditorLineHeight { get; }

        /// <summary>
        /// 編集パネルの折り返し
        /// </summary>
        public ReactiveProperty<bool> CenterTextEditorTextWrapping { get; }

        /// <summary>
        /// 編集パネルで現在行のハイライト表示
        /// </summary>
        public ReactiveProperty<bool> HighlightCurrentLine { get; }

        /// <summary>
        /// 編集パネルで行番号の表示
        /// </summary>
        public ReactiveProperty<bool> ShowLineNumbers { get; }

        /// <summary>
        /// 編集パネルでヘッダー文字列
        /// </summary>
        public ReactiveProperty<string> HeaderCharacters { get; }

        #endregion CenterTextEditor



        #region LeftTextEditor
        /// <summary>
        /// 左参照パネルフォントサイズ
        /// </summary>
        public ReactiveProperty<double> LeftTextEditorFontSize { get; }

        /// <summary>
        /// 左参照パネルフォント
        /// </summary>
        public ReactiveProperty<System.Windows.Media.FontFamily> LeftTextEditorFontFamily { get; }

        /// <summary>
        /// 左参照パネルの行間
        /// </summary>
        public ReactiveProperty<double> LeftTextEditorLineHeight { get; }

        /// <summary>
        /// 左参照パネルの折り返し
        /// </summary>
        public ReactiveProperty<bool> LeftTextEditorTextWrapping { get; }

        #endregion LeftTextEditor



        #region TopBottomTextEditor
        /// <summary>
        /// 上下参照パネルのフォントサイズ
        /// </summary>
        public ReactiveProperty<double> TopBottomTextEditorFontSize { get; }

        /// <summary>
        /// 上下参照パネルのフォント
        /// </summary>
        public ReactiveProperty<System.Windows.Media.FontFamily> TopBottomTextEditorFontFamily { get; }

        /// <summary>
        /// 上下参照パネルの行間
        /// </summary>
        public ReactiveProperty<double> TopBottomTextEditorLineHeight { get; }

        /// <summary>
        /// 上下参照パネルの折り返し
        /// </summary>
        public ReactiveProperty<bool> TopBottomTextEditorTextWrapping { get; }

        #endregion TopBottomTextEditor


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        public ConfigFlyoutViewModel()
        {
            this.Model = ConfigManager.Current.Config;

            #region General

            this.TreeTextCharacters = this.Model
                .ToReactivePropertyAsSynchronized(x => x.TreeTextCharacters)
                .AddTo(this.Disposable);

            this.NodePanelFontFamily = this.Model
               .ToReactivePropertyAsSynchronized(x => x.NodePanelFontFamily)
               .AddTo(this.Disposable);

            #endregion General

            #region CenterTextEditor

            this.CenterTextEditorFontSize = this.Model
                .ToReactivePropertyAsSynchronized(x => x.CenterTextEditorFontSize)
                .AddTo(this.Disposable);

            this.CenterTextEditorFontFamily = this.Model
                .ToReactivePropertyAsSynchronized(x => x.CenterTextEditorFontFamily)
                .AddTo(this.Disposable);

            this.CenterTextEditorLineHeight = this.Model
                .ToReactivePropertyAsSynchronized(x => x.CenterTextEditorLineHeight)
                .AddTo(this.Disposable);

            this.CenterTextEditorTextWrapping = this.Model
               .ToReactivePropertyAsSynchronized(x => x.CenterTextEditorTextWrapping)
               .AddTo(this.Disposable);

            this.HighlightCurrentLine = this.Model
               .ToReactivePropertyAsSynchronized(x => x.HighlightCurrentLine)
               .AddTo(this.Disposable);

            this.ShowLineNumbers = this.Model
               .ToReactivePropertyAsSynchronized(x => x.ShowLineNumbers)
               .AddTo(this.Disposable);

            this.HeaderCharacters = this.Model
               .ToReactivePropertyAsSynchronized(x => x.HeaderCharacters)
               .AddTo(this.Disposable);

            #endregion CenterTextEditor

            #region LeftTextEditor

            this.LeftTextEditorFontSize = this.Model
               .ToReactivePropertyAsSynchronized(x => x.LeftTextEditorFontSize)
               .AddTo(this.Disposable);

            this.LeftTextEditorFontFamily = this.Model
               .ToReactivePropertyAsSynchronized(x => x.LeftTextEditorFontFamily)
               .AddTo(this.Disposable);

            this.LeftTextEditorLineHeight = this.Model
               .ToReactivePropertyAsSynchronized(x => x.LeftTextEditorLineHeight)
               .AddTo(this.Disposable);

            this.LeftTextEditorTextWrapping = this.Model
               .ToReactivePropertyAsSynchronized(x => x.LeftTextEditorTextWrapping)
               .AddTo(this.Disposable);

            #endregion LeftTextEditor

            #region TopBottomTextEditor

            this.TopBottomTextEditorFontSize = this.Model
               .ToReactivePropertyAsSynchronized(x => x.TopBottomTextEditorFontSize)
               .AddTo(this.Disposable);

            this.TopBottomTextEditorFontFamily = this.Model
               .ToReactivePropertyAsSynchronized(x => x.TopBottomTextEditorFontFamily)
               .AddTo(this.Disposable);

            this.TopBottomTextEditorLineHeight = this.Model
               .ToReactivePropertyAsSynchronized(x => x.TopBottomTextEditorLineHeight)
               .AddTo(this.Disposable);

            this.TopBottomTextEditorTextWrapping = this.Model
               .ToReactivePropertyAsSynchronized(x => x.TopBottomTextEditorTextWrapping)
               .AddTo(this.Disposable);

            #endregion TopBottomTextEditor


            this.OpenCommand.Subscribe(_ => this.IsOpen = true);
            this.CloseCommand.Subscribe(_ => this.IsOpen = false);
            this.ApplyCommand.Subscribe(_ => this.Apply() );
            this.ResetCommand.Subscribe(_ => this.Reset() );


            this.EnlistTransaction();
        }

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
        }


        /// <summary>
        /// 開閉状態
        /// </summary>
        private bool _IsOpen = false;

        public bool IsOpen
        {
            get { return this._IsOpen; }
            set
            {
                this.SetProperty(ref this._IsOpen, value);
            }
        }

        /// <summary>
        /// 開く
        /// </summary>
        public ReactiveCommand OpenCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 閉じる
        /// </summary>
        public ReactiveCommand CloseCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 適用
        /// </summary>
        public ReactiveCommand ApplyCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// リセット
        /// </summary>
        public ReactiveCommand ResetCommand { get; private set; } = new ReactiveCommand();

        /// <summary>
        /// 適用処理
        /// </summary>
        private void Apply()
        {
            this._backings = null;
            this.CloseCommand.Execute();
        }

        /// <summary>
        /// リセット
        /// </summary>
        /// <returns></returns>
        private void Reset()
        {
            var resetModel = new Config();
            this._backings = new Dictionary<string, object>();
            foreach (PropertyInfo property in resetModel.GetType().GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(TransactionAttribute), false);
                if (!(attribute.Count() > 0 && ((TransactionAttribute)attribute[0]).IsEnlist == false))
                {
                    this._backings.Add(property.Name, property.GetValue(resetModel, null));
                }
            }
            this.Rollback();
            this.CloseCommand.Execute();
        }

        /// <summary>
        /// 元の状態に戻す
        /// </summary>
        public void Rollback()
        {
            if (this._backings != null)
            {
                foreach (PropertyInfo property in this.Model.GetType().GetProperties())
                {
                    if (this._backings.ContainsKey(property.Name))
                    {
                        property.SetValue(this.Model, this._backings[property.Name], null);
                    }
                }
                this._backings = null;
            }
        }

        /// <summary>
        /// 変更受付を開始する
        /// </summary>
        public void EnlistTransaction()
        {
            this._backings = new Dictionary<string, object>();
            foreach (PropertyInfo property in this.Model.GetType().GetProperties())
            {
                var attribute = property.GetCustomAttributes(typeof(TransactionAttribute), false);
                if (!(attribute.Count() > 0 && ((TransactionAttribute)attribute[0]).IsEnlist == false))
                {
                    this._backings.Add(property.Name, property.GetValue(this.Model, null));
                }
            }
        }

        /// <summary>
        /// Flyoutを閉じるときの処理
        /// </summary>
        public void OnOpenChanged()
        {
            if (this._backings?.Count > 0)
            {
                this.Rollback();
            }
            this.EnlistTransaction();
        }

    }
}
