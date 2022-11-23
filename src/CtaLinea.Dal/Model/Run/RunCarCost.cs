using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class RunCarCost
    {
        public Guid RunCarCostId { get; set; }
        public DateTime? StartDAte { get; set; }

        public decimal DayPrice { get; set; }
        public decimal KmPrice { get; set; }
    }
}
