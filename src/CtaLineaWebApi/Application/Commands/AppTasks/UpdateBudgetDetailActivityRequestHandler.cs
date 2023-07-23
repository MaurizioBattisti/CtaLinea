using CtaLineaWebApi.Application.Commands.Budgets;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class UpdateBudgetDetailActivityRequestHandler
        : IRequestHandler<UpdateBudgetDetailActivityRequest, bool>
    {
        private readonly ISender _mediator;
        private readonly ILogger _logger;
        public UpdateBudgetDetailActivityRequestHandler (
            ISender mediator,
            ILogger<UpdateBudgetDetailActivityRequestHandler> logger
            )
        {
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<bool> Handle(
            UpdateBudgetDetailActivityRequest request, 
            CancellationToken cancellationToken)
        {
            var myRequest = new UpdateBudgetDetailRequest()
            {
                RawRwquest = new CtaLinea.Model.Costs.CalcCostsRequest ()
                {
                    StartDate = null,
                    EndDate = null,
                    ASsociateId = null,
                    CarId = null,
                    RunId = null,
                    ContractId = null,
                    BudgetName = "Ultimo ricalcolo",
                    BudgetType = Constants.BudgetType_LastCalc,
                    ForceFreshData = true
                }
            };

            await this._mediator.Send(myRequest)
                .ConfigureAwait(false);

            return true;
        }
    }
}
