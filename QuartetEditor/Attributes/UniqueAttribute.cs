using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Attributes
{
    /// <summary>
    /// コピーの対象としないプロパティ属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property,
     AllowMultiple = false)]
    public class UniqueAttribute : Attribute
    {
    }
}
