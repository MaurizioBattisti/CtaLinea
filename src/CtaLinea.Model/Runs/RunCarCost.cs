namespace CtaLinea.Model.Runs
{
    public class RunCarCost
    {
        public Guid RunCarCostId { get; set; }
        public DateTime? StartDAte { get; set; }

        public decimal DayPrice { get; set; }
        public decimal KmPrice { get; set; }
    }
}
