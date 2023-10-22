using CtaLinea.Model.Reports;
using CtaLinea.Model.Request;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface IReportQueries
    {
        Task<IEnumerable<NegativeKmItem>> GetNegativeKmAsync(NegativeKmReportRequest request);
    }
}