using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Context
{
    public interface IZzRequestConstx
    {
        int? ContractId { get; }
        DateTime? PeriodEndDate { get; }
        DateTime? PeriodStartDate { get; }
        string UserName { get; }

        void Override(int? contractId, DateTime? startDate, DateTime? endDate);
    }
}
