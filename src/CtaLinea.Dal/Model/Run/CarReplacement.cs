using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class CarReplacement
    {
        public Guid CarReplacementId { get; set; } 

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string Note { get; set; }

        IEnumerable<Guid> OriginalPEriodCarIds { get; set; }
        IEnumerable<Guid> ReplacedPEriodCarIds { get; set; }
    }
}
