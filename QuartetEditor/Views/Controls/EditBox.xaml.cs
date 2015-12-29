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
            this.SetMarkVisibility();
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
            thisInstance.SetMarkVisibility();
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
                this.SetMarkVisibility();
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

        #region DragOverPosition

        /// <summary>
        /// ドラッグオーバーしている位置
        /// </summary>
        public DropPositionEnum DragOverPosition
        {
            get
            {
                return (DropPositionEnum)GetValue(DragOverPositionProperty);
            }
            set
            {
                SetValue(DragOverPositionProperty, value);
            }
        }

        /// <summary>
        /// DragOverPositionプロパティ
        /// </summary>
        public static readonly DependencyProperty DragOverPositionProperty =
            DependencyProperty.RegisterAttached("DragOverPosition", typeof(DropPositionEnum), typeof(EditBox),
            new PropertyMetadata(DropPositionEnum.NONE, DragOverPositionPropertyChanged));


        /// <summary>
        /// DragOverPosition変更イベント
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DragOverPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as DropPositionEnum?;
            if (flg.HasValue)
            {
                thisInstance.DragOverPosition = flg.Value;
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
            if (e.GetPosition(this).X < this.ActualWidth / 2)
            {
                
                if (e.GetPosition(this).Y < this.ActualHeight / 2)
                {
                    this.DragOverPosition = DropPositionEnum.Prev;
                }
                else
                {
                    this.DragOverPosition = DropPositionEnum.Next;
                }
            }
            else
            {
                this.DragOverPosition = DropPositionEnum.Child;

            }
            this.SetDragOverEffect();
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
        public void SetMarkVisibility()
        {
            if (this.IsReferred && this._TextBlock.Visibility == Visibility.Visible)
            {
                this._Mark.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this._Mark.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        /// <summary>
        /// ドラッグオーバー時のエフェクトを設定
        /// </summary>
        private void SetDragOverEffect()
        {
            this._TextBlockBorder.Visibility = Visibility.Hidden;
            this._InsertCursor.Visibility = Visibility.Hidden;

            if (this.IsDragOver)
            {
                switch (this.DragOverPosition)
                {
                    case DropPositionEnum.NONE:
                        break;
                    case DropPositionEnum.Prev:
                        this._InsertCursor.Visibility = Visibility.Visible;
                        this._InsertCursor.VerticalAlignment = VerticalAlignment.Top;
                        break;
                    case DropPositionEnum.Next:
                        this._InsertCursor.Visibility = Visibility.Visible;
                        this._InsertCursor.VerticalAlignment = VerticalAlignment.Bottom;
                        break;
                    case DropPositionEnum.Child:
                        this._TextBlockBorder.Visibility = Visibility.Visible;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                this._InsertCursor.Visibility = Visibility.Hidden;
            }
        }
    }
}
