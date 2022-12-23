using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Runs
{
    internal class BlCarReplacement
        : CarReplacement
    {
        public Guid RunPeriodId { get; set; }
    }
}
