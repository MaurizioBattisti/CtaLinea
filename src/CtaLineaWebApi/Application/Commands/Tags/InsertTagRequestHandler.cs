using CtaLinea.Model;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using ZzSoft.CtaLinea.Dal.Repositories;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class InsertTagRequestHandler
        : IRequestHandler<InsertTagRequest, OperationResult<int>>
    {
        private readonly ITagRepository _repo;
        private readonly ILogger _logger;

        public InsertTagRequestHandler (
            ITagRepository repo,
            ILogger<InsertTagRequestHandler> logger
            )
        {
            _repo = repo;
            _logger= logger;
        }

        public async Task<OperationResult<int>> Handle(
            InsertTagRequest request, 
            CancellationToken cancellationToken)
        {
            var id = await _repo.InsertASync(request.Model);
            return new OperationResult<int>()
            {
				Success = true,
				Data = id,
            };
        }
    }
}
