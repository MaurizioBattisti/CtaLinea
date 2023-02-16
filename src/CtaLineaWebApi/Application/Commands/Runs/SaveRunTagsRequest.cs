using CtaLinea.Model;
using MediatR;
using System;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Runs
{
	public class SaveRunTagsRequest
		: IRequest<OperationResult<bool>>
	{
		public Guid RunId { get; set; } 
		public IEnumerable<int> TagIds { get; set; }
	}
}
