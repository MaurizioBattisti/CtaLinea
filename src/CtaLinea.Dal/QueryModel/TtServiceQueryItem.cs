using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.QueryHelper;

namespace ZzSoft.CtaLinea.Dal.QueryModel
{
    [SqlAlias("s")]
    public class TtServiceQueryItem
    {
        [SqlField ("ServiceId")]
        public int Id { get; set; }
        
        [SqlField(FullText =true, SortPosition =1)]
        public string LineNumber { get; set; }
        [SqlField(FullText = true, SortPosition = 2)]
        public string RunNumber { get; set; }

        [SqlField(FullText = true)]
        public string LineDescription { get; set; }
        [SqlField(FullText = true)]
        public string PathDescription { get; set; }

        public decimal? Km { get; set; }

        public int FrequencyId { get; set; }
        [SqlField(FullText = true)]
        public string FrequencyDescription { get; set; }

        public int? TarifLineId { get; set; }
        public int? TurnOneId { get; set; }
        public int? TurnTwoId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? InsertDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public int? ServiceTypeId { get; set; }

        [SqlField(FullText = true)]
        public string StartingLocation { get; set; }
        [SqlField(FullText = true)]
        public string EndingLocation { get; set; }
        [SqlField(FullText = true)]
        public string Note { get; set; }

        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }

        [SqlField(FullText = true)]
        public string TimingDescr { get; set; }
        
        public int?  RequestedSittings { get; set; }


        [SqlField(FullText = true)]
        public string SpecReference { get; set; }
        [SqlField(FullText = true)]
        public string RunType { get; set; }
        [SqlField(FullText = true)]
        public string ServiceTypeDescr { get; set; }

        public int? VariantId { get; set; }
        public bool RunInHoursBook { get; set; }

        [SqlField(FullText = true)]
        public string Company { get; set; }
        [SqlField(FullText = true)]
        public string Company1 { get; set; }
        [SqlField(FullText = true)]
        public string Company2 { get; set; }
        [SqlField(FullText = true)]
        public string Company3 { get; set; }
        [SqlField(FullText = true)]
        public string DestinationTable { get; set; }
    }
}
