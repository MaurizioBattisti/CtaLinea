using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class RunSuspension
    {
        public Guid RunSuspensionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspensionTypeId { get; set; }

        public string SuspensionNote { get; set; }
    }
}
