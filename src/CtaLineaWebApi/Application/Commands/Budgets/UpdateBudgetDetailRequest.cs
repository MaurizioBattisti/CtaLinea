using CtaLinea.Model.Costs;
using MediatR;
using System.Collections;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class UpdateBudgetDetailRequest
        :IRequest<bool>
    {
        public CalcCostsRequest RawRwquest { get; set; }
    }
}
