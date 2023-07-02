using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Checks
{
	public class GlobalCarOverlappingItem
	{
		public Guid o_RunId { get; set; }
		public Guid o_RunVariationId { get; set; }
		public Guid RunId { get; set; }
		public Guid RunVariationId { get; set; }

		public int o_CtaRunId { get; set; }
		public string? o_ContractName { get; set; }
		public int? o_ContractRowNumber { get; set; }
		public string? o_RunName { get; set; }

		public int CtaRunId { get; set; }
		public string? ContractName { get; set; }
		public int? ContractRowNumber { get; set; }
		public string? RunName { get; set; }

		public int? o_LineNumber { get; set; }
		public string? o_RunNumber { get; set; }
		public string? o_Path { get; set; }

		public int? LineNumber { get; set; }
		public string? RunNumber { get; set; }
		public string? Path { get; set; }

        public string? o_CarType { get; set; }
        public string? CarType { get; set; }
        public TimeSpan? o_StartTime { get; set; }
        public TimeSpan? o_EndTime { get; set; }
        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public DateTime? MinDay { get; set; }
        public DateTime? MaxDay { get; set; }
        public int? DayCount { get; set; }
	}
}
