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
    class ConfigFlyoutViewModel : IDisposable
    {
        /// <summary>
        /// 設定画面を閉じるイベント
        /// </summary>
        public event Action CloseFlyout;

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
        public ReactiveProperty<string> HeaderCharactors { get; }

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

            #region CenterTextEditor

            this.CenterTextEditorFontSize = this.Model
                .ObserveProperty(x => x.CenterTextEditorFontSize)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.CenterTextEditorFontFamily = this.Model
                .ObserveProperty(x => x.CenterTextEditorFontFamily)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.CenterTextEditorLineHeight = this.Model
                .ObserveProperty(x => x.CenterTextEditorLineHeight)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.CenterTextEditorTextWrapping = this.Model
               .ObserveProperty(x => x.CenterTextEditorTextWrapping)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.HighlightCurrentLine = this.Model
               .ObserveProperty(x => x.HighlightCurrentLine)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.ShowLineNumbers = this.Model
               .ObserveProperty(x => x.ShowLineNumbers)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.HeaderCharactors = this.Model
               .ObserveProperty(x => x.HeaderCharactors)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            #endregion CenterTextEditor

            #region LeftTextEditor

            this.LeftTextEditorFontSize = this.Model
               .ObserveProperty(x => x.LeftTextEditorFontSize)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.LeftTextEditorFontFamily = this.Model
               .ObserveProperty(x => x.LeftTextEditorFontFamily)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.LeftTextEditorLineHeight = this.Model
               .ObserveProperty(x => x.LeftTextEditorLineHeight)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.LeftTextEditorTextWrapping = this.Model
               .ObserveProperty(x => x.LeftTextEditorTextWrapping)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            #endregion LeftTextEditor

            #region TopBottomTextEditor

            this.TopBottomTextEditorFontSize = this.Model
               .ObserveProperty(x => x.TopBottomTextEditorFontSize)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.TopBottomTextEditorFontFamily = this.Model
               .ObserveProperty(x => x.TopBottomTextEditorFontFamily)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.TopBottomTextEditorLineHeight = this.Model
               .ObserveProperty(x => x.TopBottomTextEditorLineHeight)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            this.TopBottomTextEditorTextWrapping = this.Model
               .ObserveProperty(x => x.TopBottomTextEditorTextWrapping)
               .ToReactiveProperty()
               .AddTo(this.Disposable);

            #endregion TopBottomTextEditor
        }

        public void Dispose()
        {
            this.Disposable.Dispose();
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

    }
}
