using CtaLinea.Model.Contab;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class InsertForfaitRequest
		: IRequest<int>
	{
		public MultiRunForfait Model { get; set; }
	}
}
