using CtaLinea.Model.Attributes;
using System.Reflection.PortableExecutable;
using System.Text;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    [SqlAlias("f")]
    public class ForfaitQueryItem
    {
        [ColumnDescription(Header = "Id")]
        [SqlField("ForfaitId")]
        public int Id { get; set; }

        [ColumnDescription(Header = "Id Appalto")]
        public int ContractId { get; set; }

        [ColumnDescription(Header = "Appalto")]
        [SqlField(FullText = true)]
        public string ContractName { get; set; } = String.Empty;

        [ColumnDescription(Header = "Nome Forfait")]
        [SqlField(FullText = true)]
        public string ForfaitName { get; set; } = string.Empty;

        [ColumnDescription(Header = "Tipo Forfait")]
        public string ForfaitTrpe { get; set; } = string.Empty;
        [ColumnDescription(Header = "Descr. Tipo Forfait")]
        [SqlField(FullText = true)]
        public string ForfaitTrpeDescr { get; set; }= string.Empty;

        [ColumnDescription(Header = "Importo")]
        public decimal Amount { get; set; }

        [ColumnDescription(Header = "Nr. Corse")]
        public int RunCount { get; set; }

        public void CopyFrom (ForfaitQueryItem item)
        {
            this.Id = item.Id;
            this.ContractId = item.ContractId;
            this.ContractName = item.ContractName;
            this.ForfaitName = item.ForfaitName;
            this.ForfaitTrpe = item.ForfaitTrpe;
            this.Amount = item.Amount;
            this.RunCount = item.RunCount;
        }
    }
}
