namespace CtaLinea.Model.Runs
{
    public class CarReplacement
    {
        public Guid CarReplacementId { get; set; } 

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Note { get; set; }

        public IList<Guid>? OriginalPEriodCarIds { get; set; }
        public IList<Guid>? ReplacedPEriodCarIds { get; set; }
    }
}
