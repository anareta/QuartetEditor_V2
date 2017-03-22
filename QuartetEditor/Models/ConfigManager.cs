using QuartetEditor.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    /// <summary>
    /// 設定情報マネージャー
    /// </summary>
    class ConfigManager
    {
        /// <summary>
        /// システムのデータ
        /// </summary>
        public static ConfigManager Current { get; } = new ConfigManager();

        /// <summary>
        /// 設定情報
        /// </summary>
        public Config Config { get; private set; } = new Config();

        /// <summary>
        /// 設定ファイル保存パス
        /// </summary>
        public string ConfigFilePath { get; private set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private ConfigManager()
        {
#if DEBUG
            this.ConfigFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Config.cnf");
#else
            this.ConfigFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), Assembly.GetEntryAssembly().GetName().Name, "Config.cnf");
#endif
        }

        /// <summary>
        /// 設定情報を読み込みます
        /// </summary>
        public void LoadConfig()
        {
            Config conf;
            if (!FileUtility.LoadJsonObject<Config>(this.ConfigFilePath, out conf))
            {
                conf = new Config();
            }
            this.Config = conf;
        }

        /// <summary>
        /// 設定情報を保存します
        /// </summary>
        public void SaveConfig()
        {
            if (!Directory.Exists(Path.GetDirectoryName(this.ConfigFilePath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(this.ConfigFilePath));
            }

            FileUtility.SaveJsonObject(this.ConfigFilePath, this.Config);
        }

    }
}
