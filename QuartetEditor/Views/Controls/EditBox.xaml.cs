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
        public EditBox()
        {
            InitializeComponent();
            this.SetBorderVisibility();
            //this.IsReferred = false;
        }

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

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.RegisterAttached("Text", typeof(string), typeof(EditBox),
            new PropertyMetadata("", TextPropertyChanged));

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

        public static readonly DependencyProperty CanEditProperty =
            DependencyProperty.RegisterAttached("CanEdit", typeof(Boolean), typeof(EditBox),
            new PropertyMetadata(false, CanEditPropertyChanged));



        public bool IsReferred
        {
            get { return (bool)GetValue(IsReferredProperty); }
            set
            {
                SetValue(IsReferredProperty, value);
                this.SetBorderVisibility();
            }
        }

        public static readonly DependencyProperty IsReferredProperty =
            DependencyProperty.RegisterAttached("IsReferred", typeof(bool), typeof(EditBox),
            new PropertyMetadata(false, IsReferredPropertyChanged));



        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var text = e.NewValue as string;
            thisInstance.TextBlock.Text = text;
        }

        private static void CanEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as bool?;
            if (flg ?? false)
            {
                thisInstance.TextEditor.Text = thisInstance.TextBlock.Text;
                thisInstance.TextEditor.Visibility = Visibility.Visible;
                thisInstance.TextBlock.Visibility = Visibility.Collapsed;

                thisInstance.TextEditor.Focus();
                thisInstance.TextEditor.SelectAll();
            }
            else
            {
                thisInstance.TextEditor.Visibility = Visibility.Collapsed;
                thisInstance.TextBlock.Visibility = Visibility.Visible;
            }
            thisInstance.SetBorderVisibility();
        }

        private static void IsReferredPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var thisInstance = d as EditBox;
            var flg = e.NewValue as bool?;
            if (flg.HasValue)
            {
                thisInstance.IsReferred = flg.Value;
            }
        }

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

        private void TextEditor_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            this.UpdateText();
            this.CanEdit = false;
        }

        private void TextEditor_LostFocus(object sender, RoutedEventArgs e)
        {
            this.UpdateText();
            this.CanEdit = false;
        }

        private void UpdateText()
        {
            if (!string.IsNullOrWhiteSpace(this.TextEditor.Text))
            {
                //this.TextBlock.Text = this.TextEditor.Text;
                this.Text = this.TextEditor.Text;
            }
        }

        public void SetBorderVisibility()
        {
            if (this.IsReferred && this.TextBlock.Visibility == Visibility.Visible)
            {
                this.Mark.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                this.Mark.Visibility = System.Windows.Visibility.Hidden;
            }
        }

    }
}
