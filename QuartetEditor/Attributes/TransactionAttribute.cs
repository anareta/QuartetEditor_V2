using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartetEditor.Attributes
{
    public class TransactionAttribute : Attribute
    {
        public bool IsEnlist { get; set; }
    }
}
