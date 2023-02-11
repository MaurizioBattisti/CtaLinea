using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("c")]
    public class CalendarQueryItem
    {
        [SqlField("CalendarId")]
        public int Id { get; set; }

		[SqlField(FullText = true, SortPosition = 2)]
        public string CalendarName { get; set; } = String.Empty;

		public int? BaseCalendarId { get; set; }
		[SqlField(FullText = true)] 
        public string? BaseCalendarName { get; set; }
        [SqlField(SortPosition = 1)]
        public int Ordinal { get; set; }

		public string CalendarType { get; set; } = String.Empty;
		[SqlField(FullText = true)]
        public string CalendarTypeDescr { get; set; } = String.Empty;

        public bool Mondays { get; set; }
        public bool Tuesdays { get; set; }
        public bool Wednesdays { get; set; }
        public bool Thursdays { get; set; }
        public bool Fridays { get; set; }
        public bool Saturdays { get; set; }
        public bool Sundays { get; set; }

        public bool PreHolyday { get; set; }
        public bool PostHolyday { get; set; }
    }
}
