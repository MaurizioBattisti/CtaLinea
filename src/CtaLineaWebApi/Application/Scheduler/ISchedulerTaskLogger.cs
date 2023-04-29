using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Scheduler
{
	public interface ISchedulerTaskLogger
	{
		int TaskId {get;set;}

		Task LogAsync(
			string message,
			string code = null);
	}
}