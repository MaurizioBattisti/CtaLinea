using CtaLinea.Model.ScheduledTasks;
using CtaLineaApp.Application.Model;

namespace CtaLineaApp.Application.Services.Base
{
    public interface IScheduledTasksService
    {
		IEnumerable<AppTaskDescription> TaskDescriptions { get; }

		Task DeleteAsync(int id);
        Task DeleteLogAsync(int id, DateTime? refDAte);
        Task<IEnumerable<ScheduledTaskItem>?> GetListAsync();
        Task<IEnumerable<SchedulerTaskLogItem>?> GetLogListAsync(int id);
        Task<ScheduledTaskItem?> GetOneAsync(int id);
        Task<int?> InsertAsync(ScheduledTaskItem item);
        Task UpdateAsync(ScheduledTaskItem item);

		Task InvokeTaskASync(
			string activityId,
			int taskId,
			object? data = null,
			int timeout = 100);
	}
}