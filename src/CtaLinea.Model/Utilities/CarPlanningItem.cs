using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Utilities
{
    public class CarPlanningItem
    {
        public Guid RunId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid CarId { get; set; }
        public string? BsCarId { get; set; }
        public string? CarDescr { get; set; }
        
        public string? RunName { get; set; }
		public int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public string? Path { get; set; }

        public string Text
        {
            get
            {
                var sb = new StringBuilder(1024);
                sb.Append(this.CarDescr ?? string.Empty);
                sb.AppendFormat(" - {0} / {1} - {2}",
                    LineNumber ?? 0,
                    RunNumber ?? string.Empty,
                    Path ?? string.Empty
                    );
                return sb.ToString();
            }
        }
    }
}
