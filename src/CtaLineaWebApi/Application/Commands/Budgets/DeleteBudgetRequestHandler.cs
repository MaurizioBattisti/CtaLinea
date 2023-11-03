using CtaLinea.Model.Costs;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Budgets
{
    public class DeleteBudgetRequestHandler
		: IRequestHandler<DeleteBudgetRequest, bool>
    {
        private readonly IBudgetRepository _repository;
        private readonly ILogger _logger;

        public DeleteBudgetRequestHandler(
            IBudgetRepository repository,
            ILogger<DeleteBudgetRequestHandler> logger
            )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(
			DeleteBudgetRequest request, 
            CancellationToken cancellationToken)
        {
            await _repository.DeleteBudgetAsync(
                request.Id)
                .ConfigureAwait(false);

            return true;
        }
    }
}
