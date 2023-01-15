using CtaLinea.Model;
using CtaLinea.Model.Runs;
using MediatR;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Abstractions;
using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class DeleteRunRequestHandler
        : IRequestHandler<DeleteRunRequest, OperationResult<Guid>>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public DeleteRunRequestHandler(
            IRunRepository runRepo,
            ILogger<GetOneRunItemRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<OperationResult<Guid>> Handle(
            DeleteRunRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<Guid>();
            result.Data = request.RunId;

            try
            {
                await _runRepo.DeleteRunAsync(
                    request.RunId)
                    .ConfigureAwait(false);

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
