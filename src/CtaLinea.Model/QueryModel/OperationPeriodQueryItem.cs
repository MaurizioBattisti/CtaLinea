using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    public class OperationPeriodQueryItem
    {
        [SqlField(SortPosition = 0, SortDirection = SortDirection.Descending)]
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public bool IsCurrent() => (this.StartDate <= DateTime.Today
                    && DateTime.Today <= this.EndDate);

        public string GetDescription() => string.Format("{0:yyyy} / {1:yyyy}",
                    this.StartDate, this.EndDate);
    }
}
