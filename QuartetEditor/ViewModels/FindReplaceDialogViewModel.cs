using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Models;
using QuartetEditor.Views.Controls;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QuartetEditor.ViewModels
{
    /// <summary>
    /// FindReplaceDialogViewModel
    /// </summary>
    public class FindReplaceDialogViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// 破棄処理
        /// </summary>
        public void Dispose()
        {
            this.Disposable.Dispose();
        }

        /// <summary>
        /// 検索情報
        /// </summary>
        private Search Model { get; set; }

        /// <summary>
        /// 「次へ」
        /// </summary>
        public ReactiveCommand FindNextCommand { get; private set; }

        /// <summary>
        /// 「前へ」
        /// </summary>
        public ReactiveCommand FindPrevCommand { get; private set; }

        /// <summary>
        /// 置換
        /// </summary>
        public ReactiveCommand ReplaceCommand { get; private set; }

        /// <summary>
        /// すべて置換
        /// </summary>
        public ReactiveCommand ReplaceAllCommand { get; private set; }

        /// <summary>
        /// 検索置換要求
        /// </summary>
        private InteractionRequest<Confirmation> _ShowSearchResultRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest ShowSearchResultRequest { get { return this._ShowSearchResultRequest; } }

        /// <summary>
        /// ダイアログ表示要求
        /// </summary>
        private InteractionRequest<Confirmation> _ShowDialogRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest ShowDialogRequest { get { return this._ShowDialogRequest; } }

        /// <summary>
        /// 検索結果のハイライト表示要求
        /// </summary>
        private InteractionRequest<Confirmation> _SearchResultHighlightRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest SearchResultHighlightRequest { get { return this._SearchResultHighlightRequest; } }

        /// <summary>
        /// テキストエディタへの参照
        /// </summary>
        private BindableTextEditor Editor { get; }

        /// <summary>
        /// FindReplaceDialogViewModel
        /// </summary>
        public FindReplaceDialogViewModel(Search model, BindableTextEditor editor)
        {
            this.Model = model;
            this.Editor = editor;

            // プロパティの設定
            this.TextToFind = this.Model
            .ToReactivePropertyAsSynchronized(x => x.TextToFind)
            .AddTo(this.Disposable);

            this.TextToReplace = this.Model
            .ToReactivePropertyAsSynchronized(x => x.TextToReplace)
            .AddTo(this.Disposable);

            this.CaseSensitive = this.Model
            .ToReactivePropertyAsSynchronized(x => x.CaseSensitive)
            .AddTo(this.Disposable);

            this.WholeWord = this.Model
            .ToReactivePropertyAsSynchronized(x => x.WholeWord)
            .AddTo(this.Disposable);

            this.UseRegex = this.Model
            .ToReactivePropertyAsSynchronized(x => x.UseRegex)
            .AddTo(this.Disposable);

            this.UseWildcards = this.Model
            .ToReactivePropertyAsSynchronized(x => x.UseWildcards)
            .AddTo(this.Disposable);

            this.WholeAllNode = this.Model
            .ToReactivePropertyAsSynchronized(x => x.WholeAllNode)
            .AddTo(this.Disposable);

            this.HilightText = this.Model
            .ToReactivePropertyAsSynchronized(x => x.HilightText)
            .AddTo(this.Disposable);

            // コマンドの設定
            this.FindNextCommand = this.TextToFind
            .Select(x => !string.IsNullOrEmpty(x))
            .ToReactiveCommand();

            this.FindNextCommand.Subscribe(_ =>
            {
                this.Model.FindNext(this.Editor.SelectionStart, this.Editor.SelectionLength, false);
            }).AddTo(this.Disposable);

            this.FindPrevCommand = this.TextToFind
            .Select(x => !string.IsNullOrEmpty(x))
            .ToReactiveCommand();

            this.FindPrevCommand.Subscribe(_ =>
            {
                if (this.Editor.IsSelected)
                {
                    this.Model.FindNext(this.Editor.SelectionStart, this.Editor.SelectionLength, true);
                }
                else
                {
                    this.Model.FindNext(this.Editor.Text.Length, 0, true);
                }
            }).AddTo(this.Disposable);

            this.ReplaceCommand = new[] {
              this.TextToFind.Select(x => !string.IsNullOrEmpty(x))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReactiveCommand();

            this.ReplaceCommand.Subscribe(_ =>
           {
               this.Model.Replace(this.Editor.SelectionStart, this.Editor.SelectionLength, this.Editor.IsSelected);
           }).AddTo(this.Disposable);


            this.ReplaceAllCommand = new[] {
              this.TextToFind.Select(x => !string.IsNullOrEmpty(x))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReactiveCommand();

            this.ReplaceAllCommand.Subscribe(_ =>
           {
               this.Model.ReplaceAll();
           }).AddTo(this.Disposable);

            this.Model.Found.Subscribe(entity =>
           {
               this._ShowSearchResultRequest.Raise(new Confirmation
               {
                   Content = entity
               });
           })
            .AddTo(this.Disposable);

            this.Model.Confirmation.Subscribe(async tuple =>
            {
                var arg = new DialogArg()
                {
                    Title = "確認",
                    Style = MahApps.Metro.Controls.Dialogs.MessageDialogStyle.AffirmativeAndNegative,
                    Message = tuple.Item1,
                    Settings = new MahApps.Metro.Controls.Dialogs.MetroDialogSettings()
                    {
                        AffirmativeButtonText = "置換",
                        NegativeButtonText = "キャンセル",
                        DefaultButtonFocus = MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative
                    }
                };

                await this._ShowDialogRequest.RaiseAsync(new Confirmation
                {
                    Content = arg
                });

                tuple.Item2(arg.Result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative);
            })
            .AddTo(this.Disposable);

            this.Model.FoundAll.Subscribe(List =>
           {
               this._SearchResultHighlightRequest.Raise(new Confirmation
               {
                   Content = List
               });
           })
           .AddTo(this.Disposable);
        }

        /// <summary>
        /// 検索文字
        /// </summary>
        public ReactiveProperty<string> TextToFind { get; }

        /// <summary>
        /// 置換文字
        /// </summary>
        public ReactiveProperty<string> TextToReplace { get; }

        /// <summary>
        /// 大文字小文字を区別する
        /// </summary>
        public ReactiveProperty<bool> CaseSensitive { get; }

        /// <summary>
        /// 単語として検索
        /// </summary>
        public ReactiveProperty<bool> WholeWord { get; }

        /// <summary>
        /// 正規表現を使用
        /// </summary>
        public ReactiveProperty<bool> UseRegex { get; }

        /// <summary>
        /// ワイルドカードを使用
        /// </summary>
        public ReactiveProperty<bool> UseWildcards { get; }

        /// <summary>
        /// 文書全体から検索
        /// </summary>
        public ReactiveProperty<bool> WholeAllNode { get; }

        /// <summary>
        /// 検索対象をハイライト表示
        /// </summary>
        public ReactiveProperty<bool> HilightText { get; }

        /// <summary>
        /// ハイライトの表示状態を更新
        /// </summary>
        public void UpdateHighlight()
        {
            this.Model.HilightAll();
        }
    }
}
