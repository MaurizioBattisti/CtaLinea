using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class DeleteTagRequestHandler
        : IRequestHandler<DeleteTagRequest, OperationResult<bool>>
    {
        private readonly ITagRepository _repo;
        private readonly ILogger _logger;

        public DeleteTagRequestHandler(
            ITagRepository repo,
            ILogger<DeleteTagRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<bool>> Handle(
            DeleteTagRequest request, 
            CancellationToken cancellationToken)
        {
            await _repo.DeleteASync(request.TagId);
            return new OperationResult<bool>()
            {
                Success = true,
                Data = true
            };
        }
    }
}
