using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models.Undo
{

    /// <summary>
    /// 実行、元に戻す(undo)、やり直す(redo)の各動作を定義するインターフェース
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// 操作を実行するメソッド
        /// </summary>
        void Do();

        /// <summary>
        /// 操作を元に戻すメソッド
        /// </summary>
        void Undo();

        /// <summary>
        /// 操作をやり直すメソッド
        /// </summary>
        void Redo();
    }

    /// <summary>
    /// 行なった操作の履歴を蓄積することでUndo,Redoの機能を提供するクラス
    /// </summary>
    public class UndoRedoManager
    {
        private Stack<ICommand> _Undo = new Stack<ICommand>();
        private Stack<ICommand> _Redo = new Stack<ICommand>();
        private bool _CanUndo = false;
        private bool _CanRedo = false;

        /// <summary>
        /// 操作を実行し、かつその内容を履歴に追加します。
        /// </summary>
        /// <param name="command">ICommandインターフェースを実装し、行う操作を定義したオブジェクト</param>
        private void Do(ICommand command)
        {
            this._Undo.Push(command);
            this.CanUndo = this._Undo.Count > 0;

            command.Do();

            this._Redo.Clear();
            this.CanRedo = this._Redo.Count > 0;
        }

        /// <summary>
        /// 操作を実行し、かつその内容を履歴に追加します。
        /// </summary>
        /// <param name="doMethod">操作を行なうメソッド</param>
        /// <param name="doParamater">doMethodに必要な引数</param>
        /// <param name="undoMethod">操作を行なう前の状態に戻すためのメソッド</param>
        /// <param name="undoParamater">undoMethodに必要な引数</param>
        public void Do(Delegate doMethod, object[] doParamater, Delegate undoMethod, object[] undoParamater)
        {
            TransactionCommand command = new TransactionCommand(doMethod, doParamater, undoMethod, undoParamater);

            this.Do(command);
        }

        /// <summary>
        /// 行なった操作を取り消してひとつ前の状態に戻します。
        /// </summary>
        public void Undo()
        {
            if (this._Undo.Count >= 1)
            {
                ICommand command = this._Undo.Pop();
                this.CanUndo = this._Undo.Count > 0;

                command.Undo();

                this._Redo.Push(command);
                this.CanRedo = this._Redo.Count > 0;
            }
        }

        /// <summary>
        /// 取り消した操作をやり直します。
        /// </summary>
        public void Redo()
        {
            if (this._Redo.Count >= 1)
            {
                ICommand command = this._Redo.Pop();
                this.CanRedo = this._Redo.Count > 0;

                command.Redo();

                this._Undo.Push(command);
                this.CanUndo = this._Undo.Count > 0;
            }
        }

        /// <summary>
        /// Undoのキャッシュをクリアします
        /// </summary>
        public void Clear()
        {
            this._Redo.Clear();
            this.CanRedo = false;
            this._Undo.Clear();
            this.CanUndo = false;
        }

        /// <summary>
        /// Undo出来るかどうかを返します。
        /// </summary>
        public bool CanUndo
        {
            private set
            {
                if (this._CanUndo != value)
                {
                    this._CanUndo = value;

                    if (this.CanUndoChange != null)
                    {
                        this.CanUndoChange.OnNext(value);
                    }
                }
            }
            get
            {
                return this._CanUndo;
            }
        }

        /// <summary>
        /// Redo出来るかどうかを返します。
        /// </summary>
        public bool CanRedo
        {
            private set
            {
                if (this._CanRedo != value)
                {
                    this._CanRedo = value;

                    if (this.CanRedoChange != null)
                    {
                        this.CanRedoChange.OnNext(value);
                    }
                }
            }
            get
            {
                return this._CanRedo;
            }
        }

        /// <summary>
        /// Undo出来るかどうかの状態が変化すると発生します。
        /// </summary>
        public Subject<bool> CanUndoChange { get; } = new Subject<bool>();

        /// <summary>
        /// Redo出来るかどうかの状態が変化すると発生します。
        /// </summary>
        public Subject<bool> CanRedoChange { get; } = new Subject<bool>();

        /// <summary>
        /// 操作
        /// </summary>
        private class TransactionCommand : ICommand
        {
            private Delegate _DoMethod;
            private Delegate _UndoMethod;
            private object[] _DoParamater;
            private object[] _UndoParamater;

            public TransactionCommand(Delegate doMethod, object[] doParamater, Delegate undoMethod, object[] undoParamater)
            {
                this._DoMethod = doMethod;
                this._DoParamater = doParamater;
                this._UndoMethod = undoMethod;
                this._UndoParamater = undoParamater;
            }

            #region ICommand メンバ

            public void Do()
            {
                _DoMethod.DynamicInvoke(_DoParamater);
            }

            public void Undo()
            {
                _UndoMethod.DynamicInvoke(_UndoParamater);
            }

            public void Redo()
            {
                _DoMethod.DynamicInvoke(_DoParamater);
            }

            #endregion
        }
    }
}
