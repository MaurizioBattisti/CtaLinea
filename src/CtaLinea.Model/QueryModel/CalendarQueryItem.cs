using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("c")]
    public class CalendarQueryItem
    {
        [SqlField("CalendarId")]
        public int Id { get; set; }

		[SqlField(FullText = true, SortPosition = 0)]
        public string CalendarName { get; set; } = String.Empty;

		public int? BaseCalendarId { get; set; }
		[SqlField(FullText = true, SortPosition = 0)] 
        public string? BaseCalendarName { get; set; }

		public string CalendarType { get; set; } = String.Empty;
		[SqlField(FullText = true, SortPosition = 0)]
        public string CalendarTypeDescr { get; set; } = String.Empty;

		public bool Sundays { get; set; }
        public bool PreHolyday { get; set; }
        public bool PostHolyday { get; set; }
    }
}
