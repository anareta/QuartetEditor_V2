using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Models
{
    public enum InsertPosition
    {
        /// <summary>
        /// 未設定
        /// </summary>
        NONE,

        /// <summary>
        /// 上に追加
        /// </summary>
        Prev,

        /// <summary>
        /// 下に追加
        /// </summary>
        Next,

        /// <summary>
        /// 子ノードに追加
        /// </summary>
        Child,
    }
}
