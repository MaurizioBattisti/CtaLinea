using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
	public class CreateRunCopyRequest
	{
		public Guid RunId { get; set; }
		public Guid? NewRunId { get; set; }
		public int? ContractRowNumber { get; set; }
		public int? LineNumber { get; set; }
		public string? RunNumber { get; set; }
		public string? RunName { get; set; }
		public string? Path { get; set; }

		public TimeSpan? StartTime { get; set; }

		public bool InvertNodes { get; set; } = false;

		public bool Inc_AdditionalDays { get; set; } = true;
		public bool Inc_ElastibusDays { get; set; } = true;
		public bool Inc_Suspensions { get; set; } = true;
		public bool Inc_Replacements { get; set; } = true;
		public bool Inc_Tabs { get; set; } = true;
		public bool Inc_Forfaits { get; set; } = true;
		public bool Inc_InternalNotes { get; set; } = true;
	}
}
