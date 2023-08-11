using CtaLinea.Model;
using CtaLinea.Model.QueryModel;
using MediatR;
using System;

namespace CtaLineaWebApi.Application.Commands.Runs
{
    public class SetRunInternalNoteRequest
        : IRequest<OperationResult<bool>>
    {
        public Guid RunId { get; set; }
        public InternalNoteQueryItem Note { get; set; } = new InternalNoteQueryItem();
    }
}
