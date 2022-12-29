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
	[SqlAlias("v")]
	public class RunVariationQueryModel
    {
		[SqlField("RunVariationId")]
        public Guid Id { get; set; }
        public Guid RunId { get; set; }

		public DateTime? VariationStartDate { get; set; }
		public int? CalendarId { get; set; }
		[SqlField(FullText = true)]
		public string? CalendarName { get; set; }
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

    }
}
