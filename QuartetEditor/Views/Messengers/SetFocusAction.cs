using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;

namespace QuartetEditor.Views.Messengers
{
    public class SetFocusAction : TriggerAction<MainWindow>
    {
        protected override void Invoke(object parameter)
        {
            // イベント引数とContextを取得する
            var args = parameter as InteractionRequestedEventArgs;
            var ctx = args.Context as Confirmation;
            var target = ctx.Content as string;

            WalkInChildren(this.AssociatedObject, (obj) =>
            {
                // プロパティの有無を確認する
                PropertyInfo propertyInfo = obj.GetType().GetProperty("Name");
                if (propertyInfo != null)
                {
                    string name = (string)propertyInfo.GetValue(obj, null);
                    if (name == target)
                    {
                        obj.Focus();
                    }
                }
            });

            // コールバックを呼び出す
            args.Callback();
        }


        /// <summary>
        /// WalkInChildrenメソッドの本体
        /// </summary>
        /// <param name="obj">検索対象</param>
        /// <param name="act">実行させる処理</param>
        private static void Walk(UIElement obj, Action<UIElement> act)
        {
            foreach (var child in LogicalTreeHelper.GetChildren(obj))
            {
                if (child is UIElement)
                {
                    act(child as UIElement);
                    Walk(child as UIElement, act);
                }
            }
        }

        /// <summary>
        /// 子オブジェクトに対してデリゲートを実行する
        /// </summary>
        /// <param name="obj">検索対象</param>
        /// <param name="act">実行させる処理</param>
        public static void WalkInChildren(UIElement obj, Action<UIElement> act)
        {
            if (act == null)
                throw new ArgumentNullException();

            Walk(obj, act);
        }

    }
}
