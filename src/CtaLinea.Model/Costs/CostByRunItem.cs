using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Costs
{
    public class CostByRunItem
    {
        public DateTime Day { get; set; }
        public Guid? AssociateId { get; set; }
        public Guid CarId { get; set; }
        public string AssociateName { get; set; } = string.Empty;
        public string CarDescription { get; set; } = string.Empty;

        public double Km { get; set; }
        public double KmExtra { get; set; }
        public decimal DayCost { get; set; }
        public decimal CostKm { get; set; }
        public decimal CostKmExtra { get; set; }

        public decimal DayIntegration { get; set; }
        public decimal DayForfait { get; set; }
        
        public decimal DayMultiRunForfait { get; set; }
        public string? MultiRunForfaitName { get; set; }
        public string? MultirunForfaitType { get; set; }
        public decimal MultiRunForfaitAmount { get; set; }

        public double KmTot => this.Km + this.KmExtra;
        public decimal Total => this.DayCost + this.CostKm + this.CostKmExtra + this.DayIntegration + this.DayForfait + this.DayMultiRunForfait;
    }
}
