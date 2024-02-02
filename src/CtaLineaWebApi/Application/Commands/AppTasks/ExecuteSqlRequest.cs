namespace CtaLineaWebApi.Application.Commands.AppTasks
{
    public class ExecuteSqlRequest
        : BaseTaskRequest
    {
        public string? SqlCommand { get; set; }
    }
}
