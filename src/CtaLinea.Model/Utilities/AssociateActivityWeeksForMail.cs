using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Utilities
{
    public class AssociateActivityWeeksForMail
    {
        public Guid AssociateId { get; set; }
        public string? AssociateDescr { get; set; }
        public string? Email { get; set; }
        public string? CarDescr { get; set; }

        public string? RunName { get; set; }
        public string? ContractName { get; set; }
        public int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public TimeSpan? StartTime { get; set; }

        public string? Path { get; set; }
        public string? RunDataDescription { get; set; }

        public DateTime? Day { get; set; }
        public string Status { get; set; } = string.Empty;

        public Guid? RunId { get; set; }
        public Guid? RunVariationId { get; set; }
    }
}
