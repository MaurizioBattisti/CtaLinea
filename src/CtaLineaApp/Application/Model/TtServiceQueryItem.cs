using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CtaLineaApp.Application.Model
{
    public class TtServiceQueryItem
    {
        public int Id { get; set; }

        public string? LineNumber { get; set; }
        public string? RunNumber { get; set; }

        public string? LineDescription { get; set; }
        public string? PathDescription { get; set; }

        public decimal? Km { get; set; }

        public int FrequencyId { get; set; }
        public string? FrequencyDescription { get; set; }

        public int? TarifLineId { get; set; }
        public int? TurnOneId { get; set; }
        public int? TurnTwoId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? InsertDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public int? ServiceTypeId { get; set; }

        public string? StartingLocation { get; set; }
        public string? EndingLocation { get; set; }
        public string? Note { get; set; }

        public DateTime? StartHour { get; set; }
        public DateTime? EndHour { get; set; }

        public string? TimingDescr { get; set; }

        public int? RequestedSittings { get; set; }


        public string? SpecReference { get; set; }
        public string? RunType { get; set; }
        public string? ServiceTypeDescr { get; set; }

        public int? VariantId { get; set; }
        public bool RunInHoursBook { get; set; }

        public string? Company { get; set; }
        public string? Company1 { get; set; }
        public string? Company2 { get; set; }
        public string? Company3 { get; set; }
        public string? DestinationTable { get; set; }
    }
}
