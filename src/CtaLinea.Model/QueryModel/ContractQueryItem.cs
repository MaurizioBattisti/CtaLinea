using CtaLinea.Model.Attributes;
using System.Reflection.PortableExecutable;
using System.Text;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("c")]
    public class ContractQueryItem
    {
        [ColumnDescription(Header = "Id", Ignore = true)]
        [SqlField("ContractId")]
        public int Id { get; set; }

        [ColumnDescription(Header = "Nome")]
        [SqlField(FullText = true, SortPosition = 2)]
        public string ContractName { get; set; } = String.Empty;

        [ColumnDescription(Header = "Descrizione")]
        [SqlField(FullText = true)]
        public string? ContractDescription { get; set; } = String.Empty;

        [ColumnDescription(Header = "Inizio")]
        [SqlField(SortPosition = 1, SortDirection = SortDirection.Descending)]
        public DateTime StartDate { get; set; }
        [ColumnDescription(Header = "Fine")]
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

        public void CopyFrom (ContractQueryItem item)
        {
            this.Id = item.Id;
            this.ContractName = item.ContractName;
            this.ContractDescription = item.ContractDescription;
            this.StartDate = item.StartDate;
            this.EndDate = item.EndDate;
        }
    }
}
