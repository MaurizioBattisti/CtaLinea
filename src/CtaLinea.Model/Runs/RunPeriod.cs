using System.Text;

namespace CtaLinea.Model.Runs
{
    public class RunPeriod
    {
        public Guid RunPeriodId { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool Monday { get; set; } = true;
        public bool Tuesday { get; set; } = true;
        public bool Wednesday { get; set; } =true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = true;
        public bool Sunday { get; set; } = true;

        public string? Note { get; set; }

        public IList<RunPeriodCar>? Cars { get; set; }

        public IList<CarReplacement>? CarReplacements { get; set; }

        public string GetWeekDaysDescr()
        {
            var daysList = new List<string>();
            if (this.Monday) daysList.Add("Lu");
            if (this.Tuesday) daysList.Add("Ma");
            if (this.Wednesday) daysList.Add("Me");
            if (this.Thursday ) daysList.Add("Gio");
            if (this.Friday) daysList.Add("Ve");
            if (this.Saturday) daysList.Add("sa");
            if (this.Sunday) daysList.Add("Dom");

            return string.Join(", ", daysList);
        }
    }

}
