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
    public class SetRunInternalNoteRequestHandler
        : IRequestHandler<SetRunInternalNoteRequest, OperationResult<bool>>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public SetRunInternalNoteRequestHandler(
            IRunRepository runRepo,
            ILogger<SetRunInternalNoteRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<OperationResult<bool>> Handle(
            SetRunInternalNoteRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<bool>()
            {
                Success = true,
                Data = true
            };

			try
            {
                result.Success =await _runRepo.SaveRunInternalNoteAsync(
                    request.RunId,
                    request.Note)
                    .ConfigureAwait(false);
			}
            catch (Exception ex)
            {
                result.Data = false;
                result.Success = false;
                result.Message = ex.Message;
			}
            return result;
		}
    }
}
