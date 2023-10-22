using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class FinalizeSimulationRequest
    {
        public string SimulationName { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }

        public bool OverwriteIfDateAlreadyExists { get; set; } = false;
    }
}
