using CtaLinea.Model.Costs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class GetCostsByRunRequestHandler
        : IRequestHandler<GetCostsByRunRequest, IEnumerable<CostByRunItem>>
    {
        private readonly IBudgetRepository _repository;
        private readonly ILogger _logger;

        public GetCostsByRunRequestHandler(
            IBudgetRepository repository,
            ILogger<GetCostsByRunRequestHandler> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<IEnumerable<CostByRunItem>> Handle(
            GetCostsByRunRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _repository.GetRunCostAsync(
                request.RawRwquest)
                .ConfigureAwait(false);

            return result;
        }
    }
}
