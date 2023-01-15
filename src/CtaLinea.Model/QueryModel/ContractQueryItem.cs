using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("c")]
    public class ContractQueryItem
    {
        [SqlField("ContractId")]
        public int Id { get; set; }

        [SqlField(FullText = true, SortPosition = 2)]
        public string ContractName { get; set; } = String.Empty;

        [SqlField(FullText = true)]
        public string? ContractDescription { get; set; } = String.Empty;

        [SqlField(FullText = true, SortPosition = 1, SortDirection = SortDirection.Descending)]
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
