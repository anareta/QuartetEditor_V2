using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor.Views.DraggableTreeView.Description
{
    /// <summary>
    /// ドラッグドロップ処理の媒介
    /// </summary>
    public sealed class DragAcceptDescription
    {
        /// <summary>
        /// DragOverイベント発生時に実行する処理
        /// </summary>
        public event Action<DragEventArgs> DragOverAction;

        /// <summary>
        /// DragOverイベント発生
        /// </summary>
        /// <param name="dragEventArgs"></param>
        public void OnOver(DragEventArgs dragEventArgs)
        {
            var handler = this.DragOverAction;
            if (handler != null)
            {
                handler(dragEventArgs);
            }
        }

        /// <summary>
        /// DragDropイベント発生時に実行する処理
        /// </summary>
        public event Action<DragEventArgs> DragDropAction;

        /// <summary>
        /// DragDropイベント発生
        /// </summary>
        /// <param name="dragEventArgs"></param>
        public void OnDrop(DragEventArgs dragEventArgs)
        {
            var handler = this.DragDropAction;
            if (handler != null)
            {
                handler(dragEventArgs);
            }
        }

        /// <summary>
        /// DragEnterイベント発生時に実行する処理
        /// </summary>
        public event Action<DragEventArgs> DragEnterAction;

        /// <summary>
        /// DragEnterイベント発生
        /// </summary>
        /// <param name="dragEventArgs"></param>
        public void OnEnter(DragEventArgs dragEventArgs)
        {
            var handler = this.DragEnterAction;
            if (handler != null)
            {
                handler(dragEventArgs);
            }
        }

        /// <summary>
        /// DragLeaveイベント発生時に実行する処理
        /// </summary>
        public event Action<DragEventArgs> DragLeaveAction;

        /// <summary>
        /// DragLeaveイベント発生
        /// </summary>
        /// <param name="dragEventArgs"></param>
        public void OnLeave(DragEventArgs dragEventArgs)
        {
            var handler = this.DragLeaveAction;
            if (handler != null)
            {
                handler(dragEventArgs);
            }
        }
    }
}
