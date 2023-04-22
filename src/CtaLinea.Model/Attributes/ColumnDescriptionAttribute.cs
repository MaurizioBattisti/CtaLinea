using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Attributes
{
    [AttributeUsage(AttributeTargets.Property,
                       AllowMultiple = false,
                       Inherited = true)]
    public class ColumnDescriptionAttribute
        : Attribute
    {
        public ColumnDescriptionAttribute () { }

        public string? Header { get; set; }
        public bool Ignore { get; set; } = false;
    }
}
