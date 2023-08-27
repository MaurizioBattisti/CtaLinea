using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Runs
{
    public class RunElastibusDay
	{
        public DateTime Day { get; set; }
        public double Km { get; set; }
        public int PeopleCount { get; set; }
        public string? Note { get; set; }
    }
}
