using System;
using System.Collections.Generic;
using System.Text;

namespace ZzSoft.CtaLinea.Dal.Utility
{
    [AttributeUsage(AttributeTargets.Property,
                       AllowMultiple = false,
                       Inherited = true)]
    class EvaluateChangesAttribute
        : Attribute
    {
        public string Message { get; set; }
        public bool Ignore { get; set; } = false;
    }
}
