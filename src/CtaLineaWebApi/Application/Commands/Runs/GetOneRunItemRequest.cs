using CtaLinea.Model.Runs;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class GetOneRunItemRequest
        : IRequest<RunItem?>
    {
        public Guid RunId { get; private set; }
        public GetOneRunItemRequest (
            Guid runId)
        {
            this.RunId = runId;
        }
    }
}
