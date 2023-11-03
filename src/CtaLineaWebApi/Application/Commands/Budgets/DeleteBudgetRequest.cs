using CtaLinea.Model.Costs;
using MediatR;
using System.Collections;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class DeleteBudgetRequest
		: IRequest<bool>
    {
        public int Id { get; set; }
    }
}
