using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuartetEditor.Utilities
{
    static class KeyboardUtility
    {
        /// <summary>
        /// コントロールキーが押されているかどうかを取得する
        /// </summary>
        static public bool IsCtrlKeyPressed
        {
            get
            {
                return (Keyboard.GetKeyStates(Key.LeftCtrl) & KeyStates.Down) == KeyStates.Down ||
                       (Keyboard.GetKeyStates(Key.RightCtrl) & KeyStates.Down) == KeyStates.Down;
            }
        }
    }
}
