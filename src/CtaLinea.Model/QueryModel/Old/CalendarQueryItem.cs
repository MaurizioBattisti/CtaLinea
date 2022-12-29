using ZzSoft.QueryHelper;

namespace CtaLinea.QueryModel
{
    [SqlAlias("c")]
    public class CalendarQueryItem
    {
        [SqlField ("CalendarId", SortPosition =0, FullText =true)]
        public string Id { get; set; }
        [SqlField(FullText = true)]
        public string Description { get; set; }
        [SqlField(FullText = true)]
        public string CalendarType { get; set; }
    }
}
