using CtaLinea.Model.Costs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class UpdateBudgetDetailRequestHandler
        : IRequestHandler<UpdateBudgetDetailRequest, bool>
    {
        private readonly IBudgetRepository _repository;
        private readonly ILogger _logger;

        public UpdateBudgetDetailRequestHandler(
            IBudgetRepository repository,
            ILogger<UpdateBudgetDetailRequestHandler> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(
            UpdateBudgetDetailRequest request, 
            CancellationToken cancellationToken)
        {
            await _repository.UpdateBudgetDetaulsAsync(
                request.RawRwquest)
                .ConfigureAwait(false);

            return true;
        }
    }
}
