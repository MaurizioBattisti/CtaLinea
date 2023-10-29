using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Dashboards
{
    public class CostDataItemRequest
	{
        public int? ContractId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool ConsiderSuspended { get; set; } = false;
        public bool RealElastibusKm { get; set; } = false;
        public string? SimulationName { get; set; }
    }
}
