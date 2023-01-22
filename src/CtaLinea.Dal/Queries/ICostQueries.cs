using CtaLinea.Model.Costs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Queries
{
    public interface ICostQueries
    {
        Task<IEnumerable<CostsByAssociate>> GetCostByAssociateAsync(int? contractId = null, DateTime? startDate = null, DateTime? endDate = null, Guid? associateId = null, Guid? carId = null, Guid? runId = null);
    }
}