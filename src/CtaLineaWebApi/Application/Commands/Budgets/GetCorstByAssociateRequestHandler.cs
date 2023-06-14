using CtaLinea.Model.Costs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class GetCorstByAssociateRequestHandler
        : IRequestHandler<GetCorstByAssociateRequest, IEnumerable<CostByAssociateItem>>
    {
        private readonly IBudgetRepository _repository;
        private readonly ILogger _logger;

        public GetCorstByAssociateRequestHandler (
            IBudgetRepository repository,
            ILogger<GetCorstByAssociateRequestHandler> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CostByAssociateItem>> Handle(
            GetCorstByAssociateRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _repository.GEtCostsByAssociateAsync(
                request.BudgetId,
                request.RawRwquest)
                .ConfigureAwait(false);

            return result;
        }
    }
}
