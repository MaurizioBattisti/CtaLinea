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
    public class CopyOneRunRequestHandler
		: IRequestHandler<CopyOneRunRequest, OperationResult<Guid>>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public CopyOneRunRequestHandler(
            IRunRepository runRepo,
            ILogger<CopyOneRunRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<OperationResult<Guid>> Handle(
			CopyOneRunRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<Guid>();

            try
            {
                var id = await _runRepo.CreateRunCopyAsync(
					request.CreateCopyPayload)
                    .ConfigureAwait(false);

                result.Success = true;
				result.Data = id;
			}
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
    }
}
