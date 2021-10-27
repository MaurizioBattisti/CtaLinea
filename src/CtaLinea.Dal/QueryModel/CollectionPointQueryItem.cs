using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("cp")]
    public class CollectionPointQueryItem
    {
        [SqlField("CollectionPointId", SortPosition =0, FullText =true)]
        public string  Id { get; set; }

        [SqlField(FullText = true)]
        public string Description { get; set; }
        [SqlField(FullText = true)]
        public string Address { get; set; }
        [SqlField(FullText = true)]
        public string City { get; set; }
        [SqlField(FullText = true)]
        public string ZipCode { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }

        [SqlField(FullText = true)]
        public string CollectionPointType { get; set; }
    }
}
