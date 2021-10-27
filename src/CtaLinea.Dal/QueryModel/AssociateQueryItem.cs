using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("a")]
    public class AssociateQueryItem
    {
        [SqlField("AssociateId")]
        public Guid Id { get; set; }

        [SqlField(SortPosition = 0, FullText = true)]
        public string Description { get; set; }
        [SqlField(FullText = true)]
        public string BsSupplierCode { get; set; }
        [SqlField(FullText = true)]
        public string BsCustomerCode { get; set; }
        [SqlField(FullText = true)]
        public string Email { get; set; }
        public bool Active { get; set; }
    }
}
