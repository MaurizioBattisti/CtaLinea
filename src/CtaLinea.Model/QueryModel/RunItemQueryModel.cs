using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
	[SqlAlias("r")]
	public class RunItemQueryModel
	{
		[SqlField("RunId")]
		public Guid Id { get; set; }
        public int CtaRunId { get; set; }

        public int? ContractId { get; set; }

		public bool Extra { get; set; }
		[SqlField(SortPosition = 0)]
        public int? ContractRowNumber { get; set; }

		[SqlField(SortPosition = 1)]
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public int? RequestedDays { get; set; }
		[SqlField(FullText = true)]
		public string? RunNote { get; set; }

		// Descrizioni
		[SqlField(SortPosition = 3, FullText = true)]
		public string? AssociatesDescr { get; set; }
		[SqlField(FullText = true)]
		public string? PrimaryCarsDescr { get; set; }
		[SqlField(FullText = true)]
		public string? SpareCarsDescr { get; set; }
		[SqlField(FullText = true)]
		public string? CalendarsDescr { get; set; }
		[SqlField(FullText = true)]
		public string? PathsDescr { get; set; }

		public Guid RunVariationId { get; set; }
        public DateTime? VariationStartDate { get; set; }
		public int? LineNumber { get; set; }
		public int? RunNumber { get; set; }
		public TimeSpan? StartTime { get; set; }
		public TimeSpan? EndTime { get; set; }

		public bool Monday { get; set; }
		public bool Tuesday { get; set; }
		public bool Wednesday { get; set; }
		public bool Thursday { get; set; }
		public bool Friday { get; set; }
		public bool Saturday { get; set; }
		public bool Sunday { get; set; }

		[SqlField(FullText = true)]
		public string? Path { get; set; }
		[SqlField(FullText = true)]
		public string? RequestedFrequency { get; set; }

		public double? Km { get; set; }
		public int? RequestedCapacity { get; set; }
		[SqlField(FullText = true)]
		public string? VariationNote { get; set; }


		public string? ContractDescription { get; set; }
		public DateTime? ContractStart { get; set; }
		public DateTime? ctrEndDAte { get; set; }

		public int VariationCount { get; set; }

    }
}
