using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class RunVariation
    {
        public Guid RunVariationId { get; set; }

        public DateTime? StartDate { get; set; }
        public  int? LineNumber { get; set; }
        public int? RunNumber { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public int? CalendarId { get; set; }

        public string RequestedFrequency { get; set; }
        public double? Km { get; set; }
        public string Note { get; set; }

        public IEnumerable<RunNode> Nodes { get; set; }
    }
}
