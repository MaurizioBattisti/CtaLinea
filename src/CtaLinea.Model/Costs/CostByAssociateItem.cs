using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Costs
{
    public class CostByAssociateItem
    {
        [ColumnDescription(Header = "Id Ditta", Ignore = true)]
        public Guid? AssociateId { get; set; }
        [ColumnDescription(Header = "Id Mezzo", Ignore = true)]
        public Guid CarId { get; set; }
        [ColumnDescription(Header = "Ditta")]
        public string AssociateName { get; set; } = string.Empty;
        [ColumnDescription(Header = "Mezzo")]
        public string CarDescription { get; set; } = string.Empty;

        [ColumnDescription(Header = "Id Appalto", Ignore = true)]
        public int ContractId { get; set; }
        [ColumnDescription(Header = "Appalto")]
        public string ContractName { get; set; } = string.Empty;


        [ColumnDescription(Header = "Mese")]
        public string Month { get; set; } = string.Empty;

        [ColumnDescription(Header = "Km")]
        public double Km { get; set; }
        [ColumnDescription(Header = "Km Extra")]
        public double KmExtra { get; set; }
        [ColumnDescription(Header = "Km Totali")]
        public double KmTot => this.Km + this.KmExtra;

        [ColumnDescription(Header = "Import al Giorno")]
        public decimal DayCost { get; set; }
        [ColumnDescription(Header = "Importo al GG per Km")]
        public decimal CostKm { get; set; }
        [ColumnDescription(Header = "Importo al GG per KM Extra")]
        public decimal CostKmExtra { get; set; }

        [ColumnDescription(Header = "Integrazione al Girono")]
        public decimal DayIntegration { get; set; }
        [ColumnDescription(Header = "Forfait al Giorno")]
        public decimal DayForfait { get; set; }

        [ColumnDescription(Header = "Forfait Multipli al giorno")]
        public decimal DayMultiRunForfait { get; set; }

        [ColumnDescription(Header = "Totale riga")]
        public decimal Total => this.DayCost + this.CostKm + this.CostKmExtra + this.DayIntegration + this.DayForfait + this.DayMultiRunForfait;

    }
}
