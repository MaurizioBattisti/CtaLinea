using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Runs
{
    internal class BlRunCarCost
        : RunCarCost
    {
        public Guid RunCarId { get; set; }
    }
}
