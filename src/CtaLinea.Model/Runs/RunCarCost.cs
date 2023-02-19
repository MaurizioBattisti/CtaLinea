using System.Runtime.CompilerServices;

namespace CtaLinea.Model.Runs
{
    public class RunCarCost
    {
        public Guid RunCarCostId { get; set; }
        public DateTime? StartDate { get; set; }

        public decimal KmPrice { get; set; }
        public decimal KmPriceExtra { get; set; }
        public decimal DayPrice { get; set; }

        public decimal? DayIntegration { get; set; }
        public decimal? DayForfait { get; set; }

        public string Text => GetCarCostHeader();

        private string GetCarCostHeader()
        {
            string header = "dall''inizio";
            if (this.StartDate != null)
            {
                header = string.Format("dal: {0:dd/MM/yyyy}", this.StartDate);
            }

            return header;
        }
    }
}
