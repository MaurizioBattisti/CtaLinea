using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Dashboards
{
    public class CarCountResult
    {
        public int NrPrimary { get; set; }
        public int NrSpare { get; set; }
        public int NrPrimarySpare { get; set; }
        public int UsedCars { get; set; }
    }
}
