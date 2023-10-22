using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class SimulationSetDayAmountsRequest
    {
        public string SimulationName { get; set; } = string.Empty;
        public decimal DayPrice { get; set; }
        public decimal? DayForfait { get;  set; }
        public decimal? DayIntegration { get; set; }
    }
}
