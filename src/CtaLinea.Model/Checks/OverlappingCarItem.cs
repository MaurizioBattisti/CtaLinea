using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Checks
{
    public class OverlappingCarItem
    {
        public int CtaRunId { get; set; }
        public string ContractName { get; set; } = string.Empty;

        public int? ContractRowNumber { get; set; }
        public string? RunName { get; set; }
        public int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public string? Path { get; set; }

        public DateTime Day { get; set; }
        public string CarType { get; set; } = string.Empty;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
