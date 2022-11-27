namespace CtaLinea.Model.Runs
{
    public class RunItem
    {
        public Guid RunId { get; set; }
        public int ContractId { get; set; } = 0;

        public bool Extra { get; set; } = false;

        public string? ContractRowNumber { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? RequestedDays { get; set; }
        
        public string? Note { get; set; }

        public IList<RunVariation>? Variations { get; set; }

        public IList<RunPeriod>? SubPeriods { get; set; }

        public IList<RunSuspension>? Suspensions { get; set; }
    }
}

