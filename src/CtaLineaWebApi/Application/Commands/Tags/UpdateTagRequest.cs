using CtaLinea.Model;
using CtaLinea.Model.Base;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Tags
{
    public class UpdateTagRequest
        : IRequest<OperationResult<bool>>
    {
        public TagForRun Model { get; set; }
    }
}
