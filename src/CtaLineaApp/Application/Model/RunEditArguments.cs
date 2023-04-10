namespace CtaLineaApp.Application.Model
{
    public class RunEditArguments
    {
        public Guid RunId { get; set; } = Guid.Empty;
        public bool IsAdding { get; set; } = false;
    }
}
