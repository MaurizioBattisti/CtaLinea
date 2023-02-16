using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class UpdateTagRequestHandler
        : IRequestHandler<UpdateTagRequest, OperationResult<bool>>
    {
        private readonly ITagRepository _repo;
        private readonly ILogger _logger;

        public UpdateTagRequestHandler(
            ITagRepository repo,
            ILogger<UpdateTagRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<bool>> Handle(
            UpdateTagRequest request, 
            CancellationToken cancellationToken)
        {
            await _repo.UpdateASync(request.Model);
            return new OperationResult<bool>()
            {
				Success = true,
				Data = true
            };
        }
    }
}
