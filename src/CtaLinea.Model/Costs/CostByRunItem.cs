using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Costs
{
    public class CostByRunItem
    {
        [ColumnDescription(Header = "Id Corsa", Ignore = true)]
        public Guid RunId { get; set; }
        [ColumnDescription(Header = "Giorno")]
        public DateTime Day { get; set; }
        [ColumnDescription(Header = "Id Ditta", Ignore = true)]
        public Guid? AssociateId { get; set; }
        [ColumnDescription(Header = "Id Mezzo", Ignore = true)]
        public Guid CarId { get; set; }
        [ColumnDescription(Header = "Ditta")]
        public string AssociateName { get; set; } = string.Empty;
        [ColumnDescription(Header = "Mezzo")]
        public string CarDescription { get; set; } = string.Empty;

        [ColumnDescription(Header = "Ora inizio")]
        public TimeSpan? StartTime { get; set; }
        [ColumnDescription(Header = "Percorso")]
        public string? Path { get; set; }
        [ColumnDescription(Header = "Frequenza")]
        public string? RequestedFrequency { get; set; }

        [ColumnDescription(Header = "Km")]
        public double Km { get; set; }
        [ColumnDescription(Header = "Km Extra")]
        public double KmExtra { get; set; }
        [ColumnDescription(Header = "Costo al GG")]
        public decimal DayCost { get; set; }
        [ColumnDescription(Header = "Costo Km GG")]
        public decimal CostKm { get; set; }
        [ColumnDescription(Header = "Costo Extra GG")]
        public decimal CostKmExtra { get; set; }

        [ColumnDescription(Header = "Integrazione GG")]
        public decimal DayIntegration { get; set; }
        [ColumnDescription(Header = "Forfait GG")]
        public decimal DayForfait { get; set; }

        [ColumnDescription(Header = "ID Forfait Multi Corsa", Ignore =true)]
        public decimal DayMultiRunForfait { get; set; }
        [ColumnDescription(Header = "Nome Forfait multi corsa")]
        public string? MultiRunForfaitName { get; set; }
        [ColumnDescription(Header = "Tipo Forfait multi corsa", Ignore = true)]
        public string? MultirunForfaitType { get; set; }
        [ColumnDescription(Header = "Forfait multi corsa al GG")]
        public decimal MultiRunForfaitAmount { get; set; }

        [ColumnDescription(Header = "Tot Km GG")]
        public double KmTot => this.Km + this.KmExtra;
        [ColumnDescription(Header = "Importo Tot. GG")]
        public decimal Total => this.DayCost + this.CostKm + this.CostKmExtra + this.DayIntegration + this.DayForfait + this.DayMultiRunForfait;
    }
}
