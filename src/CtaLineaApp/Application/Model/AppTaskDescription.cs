namespace CtaLineaApp.Application.Model
{
	public class AppTaskDescription
	{
		public string TaskId { get; set; } = string.Empty;
		public string TaskName { get; set; } = string.Empty;

		public string? DefaultArguments { get; set; }
	}
}
