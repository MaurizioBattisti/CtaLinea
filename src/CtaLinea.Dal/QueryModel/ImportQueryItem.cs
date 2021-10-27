using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("i")]
    public class ImportQueryItem
    {
        public Guid Id { get; set; }
        [SqlField("ImportDescr", FullText =true)]
        public string Description { get; set; }

        [SqlField(FullText = true)]
        public string Note { get; set; }
        [SqlField(FullText = true)]
        public string User { get; set; }

        public DateTime ImportStartDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        [SqlField(FullText = true)]
        public string ImportStatus { get; set; }

        public int ImportedElements { get; set; }
        public int ProcessedElements { get; set; }
    }
}
