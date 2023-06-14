using CtaLinea.Model.Costs;
using MediatR;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class GetCostsByRunRequest
        : IRequest<IEnumerable<CostByRunItem>>
    {
        public CalcCostsRequest RawRwquest { get; set; }
    }
}
