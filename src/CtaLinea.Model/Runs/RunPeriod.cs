namespace CtaLinea.Model.Runs
{
    public class RunPeriod
    {
        public Guid RunPEriodId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } =true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = true;
        public bool Sunday { get; set; } = true;

        public int RequestPrimaryCarCount { get; set; }

        public string? Note { get; set; }

        public IList<RunPeriodCar>? Cars { get; set; }

        public IList<CarReplacement>? CarReplacements { get; set; }
    }
}
