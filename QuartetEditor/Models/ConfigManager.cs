using System;
using System.Collections.Generic;
using System.Linq;
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
        /// コンストラクタ
        /// </summary>
        private ConfigManager()
        {

        }

        /// <summary>
        /// 設定情報を読み込みます
        /// </summary>
        public void LoadConfig()
        {

        }

        /// <summary>
        /// 設定情報を保存します
        /// </summary>
        public void SaveConfig()
        {

        }

    }
}
