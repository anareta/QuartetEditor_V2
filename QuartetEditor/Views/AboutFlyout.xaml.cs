using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
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

namespace QuartetEditor.Views
{
    /// <summary>
    /// AboutFlyout.xaml の相互作用ロジック
    /// </summary>
    public partial class AboutFlyout : UserControl
    {
        public AboutFlyout()
        {
            InitializeComponent();

            // versionを設定
            FileVersionInfo ver = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            this.VersionText.Text = "Version " + ver.FileMajorPart + "." + ver.FileMinorPart;

            // About本文を設定
            string readme;
            Assembly thisAssembly = Assembly.GetExecutingAssembly();
            using (Stream resourceStream = thisAssembly.GetManifestResourceStream("QuartetEditor.Assets.About.txt"))
            {
                using (StreamReader resourceReader = new StreamReader(resourceStream))
                {
                    readme = resourceReader.ReadToEnd();
                }
            }

            this._Message.Text = readme;
        }
    }
}
