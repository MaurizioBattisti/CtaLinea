using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Run
{
    public class RunPeriod
    {
        public Guid RunPEriodId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } =true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = false;
        public bool Sunday { get; set; } = false;

        public int RequestPrimaryCarCount { get; set; }

        public string Note { get; set; }

        public IEnumerable <RunPeriodCar> Cars { get; set; }
    }
}
