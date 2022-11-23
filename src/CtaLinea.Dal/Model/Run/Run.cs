using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class Run
    {
        public Guid RunId { get; set; }
        public int ContractId { get; set; } = 0;

        public bool Extra { get; set; } = false;

        public string ContractRowNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? RequestedDays { get; set; }
        
        public string Note { get; set; }

        public IEnumerable<RunVariation> Variations { get; set; }

        public IEnumerable<RunPeriod> SubPeriods { get; set; }

        public IEnumerable<RunSuspension> Suspensions { get; set; }
    }
}

