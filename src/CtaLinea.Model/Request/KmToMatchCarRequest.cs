using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class KmToMatchCarRequest
        : SimulationRequest
    {
        public Guid? Sim_CarMAtchId { get; set; }

        public float? MinKm { get; set; }
        public float? MaxKm { get; set; }
        public float? MinKmCContract { get; set; }
        public float? MaxKmContract { get; set; }
        public float? MinKmExtra { get; set; }
        public float? MaxKmExtra { get; set; }
    }
}
