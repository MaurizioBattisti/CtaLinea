using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Costs
{
    public class CalcCostsRequest
    {
        public int? ContractId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }


        public Guid? ASsociateId { get; set; }
        public Guid? CarId { get; set; }
        public Guid? RunId { get; set; }


        public bool IncludeOutOfPeriod { get; set; } = false;
        public bool IncludeSuspended { get; set; } = false;
        public bool UseReplacedCars { get; set; } = true;

        public string? BudgetName { get; set; }
        public string? BudgetType { get; set; }


        // indica che deve usare dati freschi
        public bool ForceFreshData { get; set; } = false;

        public bool NeedToUseFreshData ()
        {
            bool need = this.ForceFreshData;

            if (need == false)
            {
                need = (string.IsNullOrEmpty(this.BudgetName) == false);
            }
            if (need == false)
            {
                need = this.IncludeOutOfPeriod;
            }
            if (need == false)
            {
                need = this.IncludeSuspended;
            }
            if (need == false)
            {
                need = this.UseReplacedCars == false;
            }
            return need;
        }
    }
}
