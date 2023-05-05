using CtaLinea.Model.ScheduledTasks;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Repositories
{
    public interface ISchedulerTaskRepository
    {
        Task DeleteASync(int id);
        Task<int> InsertASync(ScheduledTaskItem model);
        Task UpdateASync(ScheduledTaskItem model);
    }
}