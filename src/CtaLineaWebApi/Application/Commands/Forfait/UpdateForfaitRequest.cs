using CtaLinea.Model.Contab;
using MediatR;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class UpdateForfaitRequest
		: IRequest<bool>
	{
		public int Id { get; set; }
		public MultiRunForfait Model { get; set; }
	}
}
