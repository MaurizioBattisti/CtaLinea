using System.Text;
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

        [SqlField(SortPosition = 1, SortDirection = SortDirection.Descending)]
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder(1024);
            sb.Append(this.ContractName);
            sb.Append(" ");
            if (string.IsNullOrEmpty(this.ContractDescription) == false)
            {
                sb.Append(this.ContractDescription);
                sb.Append(" ");
            }
            sb.AppendFormat("dal {0:dd/MM/yyyy} al {1:dd/MM/yyyy}",
                this.StartDate,
                this.EndDate);
            return sb.ToString();
        }
    }
}
