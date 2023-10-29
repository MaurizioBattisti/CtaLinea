using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Dashboards
{
    public class KmDataItem
    {
        public string MonthId { get; set; } = string.Empty;
        public float RealKm { get; set; }
        public float ContractKm { get; set; }
        public float WorkedContractKm { get; set; }
        public float NegativeContractKm { get; set; }
        public float ExtraKm { get; set; }
    }
}
