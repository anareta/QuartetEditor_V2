﻿using MahApps.Metro.Controls;
using QuartetEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuartetEditor.Views
{
    /// <summary>
    /// ConfigFlyout.xaml の相互作用ロジック
    /// </summary>
    public partial class ConfigFlyout : Flyout
    {
        public ConfigFlyout()
        {
            base.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            InitializeComponent();

            this.IsOpenChanged += (_, e) => 
            {
                try
                {
                    if (this.IsOpen)
                    {
                        // フォーカスの初期設定
                        this.ApplyButton.Focus();
                    }
                }
                catch
                {
                }
            };
        }
    }
}
