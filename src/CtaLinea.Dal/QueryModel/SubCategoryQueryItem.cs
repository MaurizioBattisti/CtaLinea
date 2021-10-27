using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("s")]
    public class SubCategoryQueryItem
    {
        [SqlField("SubCategoryId", SortPosition = 0, FullText = true)]
        public string Id { get; set; }
        [SqlField(FullText = true)]
        public string Description { get; set; }

        [SqlAlias("c")]
        [SqlField(FullText = true)]
        public string CategoryId { get; set; }
        [SqlAlias("c")]
        [SqlField("Description", FullText = true)]
        public string CategoryDescription { get; set; }
    }
}
