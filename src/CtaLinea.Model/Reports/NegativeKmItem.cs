using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Reports
{
    public class NegativeKmItem
    {
        [ColumnDescription(Header = "Id", Ignore = true)]
        public Guid RunId { get; set; }
        [ColumnDescription(Header = "Id CTA")]
        public int CtaRunId { get; set; }
        [ColumnDescription(Header = "Data Fine")]
        public DateTime? EndDate { get; set; }

        [ColumnDescription(Header = "Ditte")]
        public string? PrimaryAssociate { get; set; }
        [ColumnDescription(Header = "Nome corsa")]
        public string? RunName { get; set; }
        [ColumnDescription(Header = "Nr. Linea")]
        public int? LineNumber { get; set; }
        [ColumnDescription(Header = "Nr. Corsa")]
        public string? RunNumber { get; set; }
        [ColumnDescription(Header = "Percorso")]
        public string? PathDescription { get; set; }

        [ColumnDescription(Header = "Km giorno")]
        public float DayKm { get; set; }
        [ColumnDescription(Header = "GG totali")]
        public int DayCount { get; set; }
        [ColumnDescription(Header = "GG effettuati")]
        public int WorkedDayCount { get; set; }
        [ColumnDescription(Header = "GG Mancanti")]
        public int NotWorkedDayCount { get; set; }

        [ColumnDescription(Header = "Km totali")]
        public float ContractKmTotal { get; set; }
        [ColumnDescription(Header = "Km effettuati")]
        public float CetConttWorkedKm { get; set; }
        [ColumnDescription(Header = "Km Negativi")]
        public float ContractNegativeKm { get; set; }

        [ColumnDescription(Header = "Km Extra")]
        public float KmExtra { get; set; }
    }
}
