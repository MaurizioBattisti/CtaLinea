using CtaLinea.Model;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class DeleteTagRequest
        : IRequest<OperationResult<bool>>
    {
        public int TagId { get; set; }
    }
}
