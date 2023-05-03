using MediatR;
using System;
using System.Collections.Generic;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class SetDetailRequest
		:IRequest<bool>
	{
		public int? ForfaitId { get; set; }
		public IEnumerable<Guid>? RunIds { get; set; }
	}
}
