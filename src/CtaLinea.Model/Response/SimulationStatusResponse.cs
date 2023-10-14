using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Response
{
    public class SimulationStatusResponse
    {
        public int SimulatedCount { get; set; }
        public int TotalCount { get; set; }
        public double CoveragePerc {  get; set; }
    }
}
