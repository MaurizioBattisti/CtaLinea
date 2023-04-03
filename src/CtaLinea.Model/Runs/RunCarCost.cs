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

        public void CopyFrom (RunCarCost item)
        {
            this.RunCarCostId = item.RunCarCostId;
			this.StartDate = item.StartDate;

			this.KmPrice = item.KmPrice;
			this.KmPriceExtra = item.KmPriceExtra;
			this.DayPrice = item.DayPrice;

			this.DayIntegration = item.DayIntegration;
			this.DayForfait = item.DayForfait;
	    }

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
