using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using QuartetEditor.Entities;
using QuartetEditor.Models;
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
        private InteractionRequest<Confirmation> _FindReplaceRequest = new InteractionRequest<Confirmation>();

        public IInteractionRequest FindReplaceRequest { get { return this._FindReplaceRequest; } }

        /// <summary>
        /// FindReplaceDialogViewModel
        /// </summary>
        public FindReplaceDialogViewModel(Search model)
        {
            this.Model = model;

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

            // コマンドの設定
            this.FindNextCommand = this.TextToFind
            .Select(x => !string.IsNullOrEmpty(x))
            .ToReactiveCommand();

            this.FindNextCommand.Subscribe( _ =>
            {
                this.Model.FindNext();
            }).AddTo(this.Disposable);

            this.FindPrevCommand = this.TextToFind
            .Select(x => !string.IsNullOrEmpty(x))
            .ToReactiveCommand();

            this.FindPrevCommand.Subscribe(_ =>
            {
                this.Model.FindPrev();
            }).AddTo(this.Disposable);

            this.ReplaceCommand = new[] {
              this.TextToFind.Select(x => !string.IsNullOrEmpty(x)),
              this.TextToReplace.Select(x => !string.IsNullOrEmpty(x))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReactiveCommand();

            this.ReplaceCommand.Subscribe( _ =>
            {
                this.Model.Replace();
            }).AddTo(this.Disposable);


            this.ReplaceAllCommand = new[] {
              this.TextToFind.Select(x => !string.IsNullOrEmpty(x)),
              this.TextToReplace.Select(x => !string.IsNullOrEmpty(x))
            }
            .CombineLatestValuesAreAllTrue()
            .ToReactiveCommand();

            this.ReplaceAllCommand.Subscribe( _ =>
            {
                this.Model.ReplaceAll();
            }).AddTo(this.Disposable);

            this.Model.FindReplaceRequest.Subscribe(async entity =>
            {
               await this._FindReplaceRequest.RaiseAsync(new Confirmation
               {
                   Content = entity
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

    }
}
