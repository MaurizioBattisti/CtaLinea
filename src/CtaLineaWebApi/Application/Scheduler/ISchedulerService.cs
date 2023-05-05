using CtaLinea.Model.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CtaLineaWebApi.Application.Scheduler
{
	public interface ISchedulerService
	{
        Task StartActivityAsync(string activityId, int id);
		Task EndActivityAsycn(string activityId, int id);
		Task ReloadSchedulerAsync();

        IEnumerable<ScheduledTaskItem> GetCurrentSchedulerTasks();
        Task<IEnumerable<SchedulerTaskLogItem>> GetTaskLogAsync(
			int id);
        Task CleanTaskLogAsync(
            int id,
            DateTime? refDate = null);

    }
}