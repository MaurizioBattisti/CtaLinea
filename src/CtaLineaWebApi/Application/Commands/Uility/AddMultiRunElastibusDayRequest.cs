using CtaLinea.Model.Response;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Uility
{
	public class AddMultiRunElastibusDayRequest
		: IRequest<OperationResponse>
	{
		public string? Data { get; set; }
	}
}
