using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Contab
{
    public class MultiRunForfait
    {
        private const string ForfaitTYpe_Day = "D";
        private const string ForfaitTYpe_Month = "M";
        private const string ForfaitTYpe_Year = "Y";

        public int ForfaitId { get; set; }
        public int ContractId { get; set; }
        public string ForfaitName { get; set; } = string.Empty;
        public string ForfaitTrpe { get; set; } = string.Empty;
        #region forfait type as enumeration
        public ForfaitTrpeEnum ForfaitType2 
        {
            get
            {
                ForfaitTrpeEnum result = ForfaitTrpeEnum.Daily;
                switch (this.ForfaitTrpe)
                {
                    case ForfaitTYpe_Day:
                        result = ForfaitTrpeEnum.Daily;
                        break;
                    case ForfaitTYpe_Month:
                        result = ForfaitTrpeEnum.Monthly;
                        break;
                    case ForfaitTYpe_Year:
                        result = ForfaitTrpeEnum.Yearly;
                        break;
                }
                return result;
            }
            set
            {
                switch (value)
                {
                    case ForfaitTrpeEnum.Daily:
                        this.ForfaitTrpe = ForfaitTYpe_Day;
                        break;
                    case ForfaitTrpeEnum.Monthly:
                        this.ForfaitTrpe = ForfaitTYpe_Month;
                        break;
                    case ForfaitTrpeEnum.Yearly:
                        this.ForfaitTrpe = ForfaitTYpe_Year;
                        break;
                    default:
                        this.ForfaitTrpe = ForfaitTYpe_Day;
                        break;
                }
            }
        }
        #endregion

        public decimal Amount { get; set; }

        public IEnumerable<Guid>? RunIds { get; set; }
    }

    public enum ForfaitTrpeEnum
    {
        Daily,
        Monthly,
        Yearly
    }
}
