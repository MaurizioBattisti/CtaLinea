namespace CtaLinea.Model.Runs
{
    public class RunCarCost
    {
        public Guid RunCarCostId { get; set; }
        public DateTime? StartDate { get; set; }

        public decimal KmPrice { get; set; }
		public decimal DayPrice { get; set; }

		public decimal? DayIntegration { get; set; }
		public decimal? DayForfait { get; set; }
	}
}
