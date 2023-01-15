using System;
using System.Collections.Generic;
using System.Text;

namespace ZzSoft.QueryHelper
{
    [AttributeUsage(AttributeTargets.Property,
                       AllowMultiple = false,
                       Inherited = true)]
    public class SqlFieldAttribute
        : Attribute
    {
        public SqlFieldAttribute () { }
        public SqlFieldAttribute(string name) 
        {
            this.Name = name;
        }

        public string Name { get; set; } = string.Empty;
        public int SortPosition { get; set; } = -1;
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public bool FullText { get; set; }
    }
}
