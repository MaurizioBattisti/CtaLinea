using CtaLinea.Model.Base;

namespace CtaLinea.Model.Runs
{
    public class RunVariation
    {
        public Guid RunVariationId { get; set; }

        public DateTime? StartDate { get; set; }
        public  int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public string Path { get; set; } = string.Empty;

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }

        public IList<int>? Calendars { get; set; }
        public IList<int>? ExclusionCalendars { get; set; }

        public bool Monday { get; set; } = true;
		public bool Tuesday { get; set; } = true;
		public bool Wednesday { get; set; } = true;
		public bool Thursday { get; set; } = true;
		public bool Friday { get; set; } = true;
		public bool Saturday { get; set; } = true;
		public bool Sunday { get; set; } = true;

		public string? RequestedFrequency { get; set; }

        public int RequestedCapacity { get; set; }
		public double? Km { get; set; }
        public string? Note { get; set; }

        public IList<RunNode>? Nodes { get; set; }
    }
}
