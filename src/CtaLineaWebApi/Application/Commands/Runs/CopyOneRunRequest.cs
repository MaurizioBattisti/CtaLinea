using CtaLinea.Model;
using CtaLinea.Model.Request;
using CtaLinea.Model.Runs;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class CopyOneRunRequest
		: IRequest<OperationResult<Guid>>
    {
        public CreateRunCopyRequest CreateCopyPayload { get; set; }
    }
}
