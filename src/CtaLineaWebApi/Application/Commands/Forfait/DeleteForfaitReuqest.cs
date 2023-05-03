using MediatR;

namespace CtaLineaWebApi.Application.Commands.Forfait
{
	public class DeleteForfaitReuqest
		: IRequest<bool>
	{
		public int Id { get; set; }
	}
}
