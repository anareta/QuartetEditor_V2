using QuartetEditor.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuartetEditor.Views.Controls
{
    /// <summary>
    /// EditBox.xaml の相互作用ロジック
    /// </summary>
    public partial class EditBox : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EditBox()
        {
            InitializeComponent();
            this.SetRefferdMark();
        }

        #region Text
        /// <summary>
        /// 表示する文字列
        /// </summary>
        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Textプロパティ
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(EditBox),
            new PropertyMetadata("", TextPropertyChanged));

        /// <summary>
        /// Textプロパティ変更処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var text = e.NewValue as string;
            thisInstance._TextBlock.Text = text;
        }

        #endregion Text

        #region CanEdit
        /// <summary>
        /// 編集可能か否か
        /// </summary>
        public bool CanEdit
        {
            get
            {
                return (bool)GetValue(CanEditProperty);
            }
            set
            {
                SetValue(CanEditProperty, value);
            }
        }

        /// <summary>
        /// CanEditプロパティ
        /// </summary>
        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.RegisterAttached("CanEdit", typeof(Boolean), typeof(EditBox),
            new PropertyMetadata(false, CanEditPropertyChanged));

        /// <summary>
        /// CanEdit変更処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void CanEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as bool?;
            if (flg ?? false)
            {
                thisInstance._TextEditor.Text = thisInstance._TextBlock.Text;
                thisInstance._TextEditor.Visibility = Visibility.Visible;
                thisInstance._TextBlock.Visibility = Visibility.Collapsed;

                thisInstance._TextEditor.Focus();
                thisInstance._TextEditor.SelectAll();
            }
            else
            {
                thisInstance._TextEditor.Visibility = Visibility.Collapsed;
                thisInstance._TextBlock.Visibility = Visibility.Visible;
            }
            thisInstance.SetRefferdMark();
        }

        #endregion CanEdit

        #region IsReferred

        /// <summary>
        /// 参照状態にあるか否か
        /// </summary>
        public bool IsReferred
        {
            get { return (bool)GetValue(IsReferredProperty); }
            set
            {
                SetValue(IsReferredProperty, value);
                this.SetRefferdMark();
            }
        }

        /// <summary>
        /// IsReferredプロパティ
        /// </summary>
        public static readonly DependencyProperty IsReferredProperty =
            DependencyProperty.RegisterAttached("IsReferred", typeof(bool), typeof(EditBox),
            new PropertyMetadata(false, IsReferredPropertyChanged));

        /// <summary>
        /// IsReferredプロパティ変更処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsReferredPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as bool?;
            if (flg.HasValue)
            {
                thisInstance.IsReferred = flg.Value;
            }
        }

        #endregion IsReferred


        #region IsDragOver

        /// <summary>
        /// マウスでドラッグオーバーしているか否か
        /// </summary>
        public bool IsDragOver
        {
            get { return (bool)GetValue(IsDragOverProperty); }
            set
            {
                SetValue(IsDragOverProperty, value);
            }
        }

        /// <summary>
        /// IsDragOverプロパティ
        /// </summary>
        public static readonly DependencyProperty IsDragOverProperty =
            DependencyProperty.RegisterAttached("IsDragOver", typeof(bool), typeof(EditBox),
            new PropertyMetadata(false, IsDragOverPropertyChanged));

        /// <summary>
        /// IsDragOverプロパティ変更処理
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsDragOverPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as bool?;
            if (flg.HasValue)
            {
                thisInstance.IsDragOver = flg.Value;
                thisInstance.SetDragOverEffect();
            }
        }

        #endregion IsDragOver

        #region DropPosition

        /// <summary>
        /// ドラッグオーバーしている位置
        /// </summary>
        public DropPositionEnum DropPosition
        {
            get
            {
                return (DropPositionEnum)GetValue(DropPositionProperty);
            }
            set
            {
                SetValue(DropPositionProperty, value);
                this.SetDragOverEffect();
            }
        }

        /// <summary>
        /// DropPositionプロパティ
        /// </summary>
        public static readonly DependencyProperty DropPositionProperty =
            DependencyProperty.RegisterAttached("DropPosition", typeof(DropPositionEnum), typeof(EditBox),
            new PropertyMetadata(DropPositionEnum.NONE, DropPositionPropertyChanged));


        /// <summary>
        /// DropPosition変更イベント
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DropPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as DropPositionEnum?;
            if (flg.HasValue)
            {
                thisInstance.DropPosition = flg.Value;
            }
        }
        #endregion DragOverPosition

        #region Event

        /// <summary>
        /// 編集中のキー処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Return:
                    this.UpdateText();
                    this.CanEdit = false;
                    break;
                case Key.Escape:
                    this.CanEdit = false;
                    break;
            }
        }

        /// <summary>
        /// 編集中のフォーカス処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.UpdateText();
            this.CanEdit = false;
        }

        /// <summary>
        /// 編集中のフォーカス処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateText();
            this.CanEdit = false;
        }

        /// <summary>
        /// ドラッグオーバーイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UserControl_PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.GetPosition(this).X < (this.ActualWidth*0.7))
            {
                if (e.GetPosition(this).Y < this.ActualHeight / 2)
                {
                    this.DropPosition = DropPositionEnum.Prev;
                }
                else
                {
                    this.DropPosition = DropPositionEnum.Next;
                }
            }
            else
            {
                this.DropPosition = DropPositionEnum.Child;
            }
        }

        #endregion Event

        /// <summary>
        /// 入力されたテキストを反映する
        /// </summary>
        private void UpdateText()
        {
            if (!string.IsNullOrWhiteSpace(this._TextEditor.Text))
            {
                this.Text = this._TextEditor.Text;
            }
        }

        /// <summary>
        /// 参照マークを変更する
        /// </summary>
        public void SetRefferdMark()
        {
            if (this.IsReferred && this._TextBlock.Visibility == Visibility.Visible)
            {
                this._TextBlock.Visibility = Visibility.Collapsed;
                this._TextBlockReferred.Visibility = Visibility.Visible;
            }
            else
            {
                this._TextBlock.Visibility = Visibility.Visible;
                this._TextBlockReferred.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// ドラッグオーバー時のエフェクトを設定
        /// </summary>
        private void SetDragOverEffect()
        {
            this._TextBlockBorder.Visibility = Visibility.Hidden;
            this.HideAdorner();

            if (this.IsDragOver)
            {
                switch (this.DropPosition)
                {
                    case DropPositionEnum.NONE:
                        break;
                    case DropPositionEnum.Prev:
                        this.ShowAdorner(VerticalAlignment.Top);
                        break;
                    case DropPositionEnum.Next:
                        this.ShowAdorner(VerticalAlignment.Bottom);
                        break;
                    case DropPositionEnum.Child:
                        this._TextBlockBorder.Visibility = Visibility.Visible;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            else
            {

            }
        }

        #region InsertionCursor
        /// <summary>
        /// 挿入位置カーソル
        /// </summary>
        private InsertionAdorner _InsertionAdorner;

        /// <summary>
        /// 挿入位置カーソルを表示します
        /// </summary>
        /// <param name="position"></param>
        private void ShowAdorner(VerticalAlignment position)
        {
            this._InsertionAdorner = new InsertionAdorner(this, position);
        }

        /// <summary>
        /// 挿入位置カーソルを消します
        /// </summary>
        private void HideAdorner()
        {
            if (this._InsertionAdorner != null)
            {
                this._InsertionAdorner.Detach();
                this._InsertionAdorner = null;
            }
        }

        #endregion InsertionCursor
    }
}
