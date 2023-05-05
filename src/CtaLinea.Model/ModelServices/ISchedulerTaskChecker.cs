using CtaLinea.Model.Helpers;
using CtaLinea.Model.ScheduledTasks;

namespace CtaLinea.Model.ModelServices
{
    public interface ISchedulerTaskChecker
    {
        Task<CheckResult> CheckASync(ScheduledTaskItem item);
    }
}