using CtaLinea.Model.Base;

namespace CtaLinea.Model.Runs
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
        public Calendar? CalendarData { get; set; }

        public string? RequestedFrequency { get; set; }

        public double? Km { get; set; }
        public string? Note { get; set; }

        public IList<RunNode>? Nodes { get; set; }
    }
}
