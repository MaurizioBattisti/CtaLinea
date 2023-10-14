using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class SimulationRequest
    {
        public string SimulationName { get; set; } = string.Empty;
        public int? ContractId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
}
