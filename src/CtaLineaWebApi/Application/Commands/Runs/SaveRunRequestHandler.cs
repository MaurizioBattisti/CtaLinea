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
    public class SaveRunRequestHandler
        : IRequestHandler<SaveRunRequest, OperationResult<Guid>>
    {
        private readonly IRunRepository _runRepo;
        private readonly ILogger _logger;

        public SaveRunRequestHandler(
            IRunRepository runRepo,
            ILogger<GetOneRunItemRequestHandler> logger)
        {
            _runRepo = runRepo;
            _logger = logger;
        }

        public async Task<OperationResult<Guid>> Handle(
            SaveRunRequest request, 
            CancellationToken cancellationToken)
        {
            var result = new OperationResult<Guid>();
            var model = request.RunItem;
            if (request.RunId != null) model.RunId = request.RunId.Value;
            if (model.RunId == Guid.Empty ) model.RunId= Guid.NewGuid();

            result.Data = model.RunId;

            try
            {
                await _runRepo.SaveRuAsync(
                    model)
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
