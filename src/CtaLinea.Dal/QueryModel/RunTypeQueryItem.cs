using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("t")]
    public class RunTypeQueryItem
    {
        [SqlField("RunTypeId")]
        public string  Id { get; set; }

        [SqlField(SortPosition = 0, FullText = true)]
        public string Description { get; set; }
    }
}
