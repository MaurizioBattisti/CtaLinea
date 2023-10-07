namespace CtaLinea.Model.Runs
{
    public class RunForPrintModel
    {
        public Guid  RunId { get; set; }
		public int? ContractId { get; set; }
        public string? ContractName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public int CtaRunId { get; set; }
        public bool Extra { get; set; }
        public int? ContractRowNumber { get; set; }
        public bool  Elastibus { get; set; }
        public string? PrimaryAssociateDescr { get; set; }
        public string? PrimaryCarDescr { get; set; }
        public string? SpareAssociateDescr { get; set; }
        public string? SpareCarDescr { get; set; }

        public int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public string? RunName { get; set; }
        public string? RequestedFrequency { get; set; }
        public float? Km { get; set; }
        public int? RequestedCapacity { get; set; }

        public string? Inc_CalendarDescr { get; set; }
        public string? Exc_CalendarDescr { get; set; }

		// primo nodo
        public TimeSpan? Fn_Hour { get; set; }
        public string? Fn_CollectionPointId { get; set; }
        public string? Fn_cpDescr { get; set; }
        public string? Fn_CoincidenceDescr { get; set; }

        // ultimo nodo
        public TimeSpan? Ln_Hour { get; set; }
        public string? Ln_CollectionPointId { get; set; }
        public string? Ln_cpDescr { get; set; }
        public string? Ln_CoincidenceDescr { get; set; }
    }
}
