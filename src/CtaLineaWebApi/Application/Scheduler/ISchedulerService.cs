using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Scheduler
{
	public interface ISchedulerService
	{
		Task StartActivityAsync(string activityId, int id);
		Task EndActivityAsycn(string activityId, int id);
		Task ReloadSchedulerAsync();
	}
}