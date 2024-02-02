using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Request
{
    public class SetRunLockRequest
    {
        public DateTime? Date { get; set; } = null;
        public string? Note { get; set; } = null;
        public IEnumerable<Guid>? RunIds { get; set; }
    }
}
