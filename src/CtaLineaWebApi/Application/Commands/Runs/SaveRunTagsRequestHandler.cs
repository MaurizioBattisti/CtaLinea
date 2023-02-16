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
    public class SaveRunTagsRequestHandler
		: IRequestHandler<SaveRunTagsRequest, OperationResult<bool>>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public SaveRunTagsRequestHandler(
            IRunRepository runRepo,
            ILogger<SaveRunTagsRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<OperationResult<bool>> Handle(
			SaveRunTagsRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<bool>()
            {
                Success = true,
                Data = true
            };

			try
            {
                await _runRepo.SaveRunTagsAsync(
                    request.RunId,
                    request.TagIds);
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
