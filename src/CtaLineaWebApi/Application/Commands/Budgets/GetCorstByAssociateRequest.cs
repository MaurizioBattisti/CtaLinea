using CtaLinea.Model.Costs;
using MediatR;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class GetCorstByAssociateRequest
        : IRequest<IEnumerable<CostByAssociateItem>>
    {
        public int? BudgetId { get; set; }
        public CalcCostsRequest RawRwquest { get; set; }
    }
}
