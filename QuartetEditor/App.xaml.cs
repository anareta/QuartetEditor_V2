using QuartetEditor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuartetEditor
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

            // 設定ファイルの読み込み
            ConfigManager.Current.LoadConfig();
        }

        /// <summary>
        /// 集約エラーハンドラ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(
                "不明なエラーが発生しました。アプリケーションを終了します。",
                "エラー",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            try
            {
                var model = QEDocument.Current;
                if (model.IsEdited && !string.IsNullOrWhiteSpace(model.FilePath))
                {
                    // 未保存の場合、保存を試みる
                    string tmpFilePath = "";
                    int count = 0;

                    while ((string.IsNullOrWhiteSpace(tmpFilePath) || File.Exists(tmpFilePath)) && count++ < 10)
                    {
                        // 重複しないファイル名が出るまでGetRandomFileNameを呼び続ける
                        tmpFilePath = Path.Combine(Path.GetDirectoryName(model.FilePath),
                                                   Path.GetFileNameWithoutExtension(model.FilePath) + "_" + Path.GetRandomFileName() + ".txt");
                    }

                    model.Save(tmpFilePath);
                }
            }
            catch
            {

            }

            Environment.Exit(1);
        }
    }
}
