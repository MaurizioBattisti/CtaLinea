namespace CtaLineaApp.Pages.Scheduler.AppTasks
{
    public class AppTaskDescription
    {
        public string TaskId { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;

        public Type TaskHandlerType { get; set; } = typeof(object);
    }
}
