using System;
using System.Collections.Generic;
using System.Text;

namespace ZzSoft.QueryHelper
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class,
                       AllowMultiple = false,
                       Inherited = true)]
    public class SqlAliasAttribute
        : Attribute
    {
        public SqlAliasAttribute() { }
        public SqlAliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        public string Alias { get; set; } = string.Empty;
    }
}
