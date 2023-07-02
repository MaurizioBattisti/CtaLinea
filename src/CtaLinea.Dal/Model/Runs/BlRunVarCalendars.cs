using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Runs
{
    internal class BlRunVarCalendars
    {
        public int CalendarId { get; set; }
        public Guid RunVariationId { get; set; }
        public bool Exclusion { get; set; } = false;
    }
}
