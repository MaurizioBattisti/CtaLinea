using ZzSoft.QueryHelper;

namespace CtaLinea.QueryModel
{
    public class ImportDetailQueryItem
        : TtServiceQueryItem
    {
        [SqlAlias("d")]
        public Guid ImportId { get; set; }
        [SqlAlias("d")]
        public int ServiceId { get; set; }

        [SqlAlias("d")]
        [SqlField("Note", FullText = true)]
        public string ImportNote { get; set; }
    }
}
