using ICSharpCode.AvalonEdit.Document;
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
    class NodeViewModel : IDisposable
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


        public NodeViewModel(Node model)
        {
            this.Model = model;

            this.Name = this.Model
                .ObserveProperty(x => x.Name)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.ID = this.Model
                .ObserveProperty(x => x.ID)
                .ToReactiveProperty()
                .AddTo(this.Disposable);

            this.Content = this.Model
                .ObserveProperty(x => x.Content)
                .ToReactiveProperty()
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
