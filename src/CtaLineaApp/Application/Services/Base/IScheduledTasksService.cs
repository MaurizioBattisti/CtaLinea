using CtaLinea.Model.ScheduledTasks;

namespace CtaLineaApp.Application.Services.Base
{
    public interface IScheduledTasksService
    {
        Task DeleteAsync(int id);
        Task DeleteLogAsync(int id, DateTime? refDAte);
        Task<IEnumerable<ScheduledTaskItem>?> GetListAsync();
        Task<IEnumerable<SchedulerTaskLogItem>?> GetLogListAsync(int id);
        Task<ScheduledTaskItem?> GetOneAsync(int id);
        Task<int?> InsertAsync(ScheduledTaskItem item);
        Task UpdateAsync(ScheduledTaskItem item);
    }
}