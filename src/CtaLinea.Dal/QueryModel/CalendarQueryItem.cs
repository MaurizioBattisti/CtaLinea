using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("c")]
    public class CalendarQueryItem
    {
        [SqlField ("CalendarId", SortPosition =0, FullText =true)]
        public string Id { get; set; }
        [SqlField(FullText = true)]
        public string Description { get; set; }
        [SqlField(FullText = true)]
        public string CalendarType { get; set; }
    }
}
