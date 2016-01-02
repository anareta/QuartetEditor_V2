using ICSharpCode.AvalonEdit.Document;
using Prism.Mvvm;
using QuartetEditor.Enums;
using QuartetEditor.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.ViewModels
{
    class NodeViewModel : BindableBase, IDisposable
    {
        /// <summary>
        /// 破棄用
        /// </summary>
        private CompositeDisposable Disposable { get; } = new CompositeDisposable();

        /// <summary>
        /// モデルクラス
        /// </summary>
        public Node Model { get; }

        /// <summary>
        /// 識別番号
        /// </summary>
        public ReactiveProperty<string> ID { get; }

        /// <summary>
        /// ノード名
        /// </summary>
        public ReactiveProperty<string> Name { get; }

        /// <summary>
        /// ノードのコンテンツ
        /// </summary>
        public ReactiveProperty<TextDocument> Content { get; }

        /// <summary>
        /// 子要素
        /// </summary>
        public ReadOnlyReactiveCollection<NodeViewModel> Children { get; }

        /// <summary>
        /// 展開状態
        /// </summary>
        public ReactiveProperty<bool> IsExpanded { get; }

        /// <summary>
        /// 選択状態
        /// </summary>
        public ReactiveProperty<bool> IsSelected { get; }

        /// <summary>
        /// ノード名編集モード
        /// </summary>
        public ReactiveProperty<bool> IsNameEditMode { get; }

        /// <summary>
        /// 参照状態
        /// </summary>
        public ReactiveProperty<bool> IsReferred { get; }

        /// <summary>
        /// ドラッグオーバーしているか
        /// </summary>
        public ReactiveProperty<bool> IsDragOver { get; }

        /// <summary>
        /// ドロップする位置
        /// </summary>
        public ReactiveProperty<DropPositionEnum> DropPosition { get; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="model"></param>
        public NodeViewModel(Node model)
        {
            this.Model = model;

            this.Name = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Name)
                .AddTo(this.Disposable);

            this.ID = this.Model
                .ToReactivePropertyAsSynchronized(x => x.ID)
                .AddTo(this.Disposable);

            this.Content = this.Model
                .ToReactivePropertyAsSynchronized(x => x.Content)
                .AddTo(this.Disposable);

            this.IsExpanded = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsExpanded)
                .AddTo(this.Disposable);

            this.IsSelected = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsSelected)
                .AddTo(this.Disposable);

            this.IsNameEditMode = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsNameEditMode)
                .AddTo(this.Disposable);

            this.IsReferred = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsReferred)
                .AddTo(this.Disposable);

            this.IsDragOver = this.Model
                .ToReactivePropertyAsSynchronized(x => x.IsDragOver)
                .AddTo(this.Disposable);

            this.DropPosition = this.Model
                .ToReactivePropertyAsSynchronized(x => x.DropPosition)
                .AddTo(this.Disposable);

            this.Children = this.Model
                .Children
                .ToReadOnlyReactiveCollection(x => new NodeViewModel(x));
        }


        public void Dispose()
        {
            this.Disposable.Dispose();
        }
    }
}
