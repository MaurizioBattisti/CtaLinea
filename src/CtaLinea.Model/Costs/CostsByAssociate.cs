using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Costs
{
    public class CostsByAssociate
    {
        public string ContractName { get; set; } = string.Empty;
        public string AssociateDescr { get; set; } = string.Empty;
        public string CarDescr { get; set; } = string.Empty;
        
        public int ContractId { get; set; }
        public Guid AssociateId { get; set; }
        public Guid CarId { get; set; }

        public float Tot_Km { get; set; }
        public decimal Tot_KmCost { get; set; }
        public decimal Tot_DayPrice { get; set; }
        public decimal Tot_DayForfait { get; set; }
        public decimal Tot_DayIntegration { get; set; }
        public decimal Tot { get; set; }
    }
}
