using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    public class ImportDetailQueryItem
        : TtServiceQueryItem
    {
        [SqlAlias("d")]
        public Guid ImportId { get; set; }
        [SqlAlias("d")]
        public int ServiceId { get; set; }

        [SqlAlias("d")]
        [SqlField("Note", FullText = true)]
        public string ImportNote { get; set; }
    }
}
