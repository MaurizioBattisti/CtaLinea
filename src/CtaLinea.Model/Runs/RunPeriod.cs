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
        public bool Wednesday { get; set; } = true;
        public bool Thursday { get; set; } = true;
        public bool Friday { get; set; } = true;
        public bool Saturday { get; set; } = true;
        public bool Sunday { get; set; } = true;

        public string? Note { get; set; }

        public IList<RunPeriodCar>? Cars { get; set; }

        public IList<CarReplacement>? CarReplacements { get; set; }

        public string Text => GetWeekDaysDescr();
        public string GetDaysHash()
        {
            var sb = new StringBuilder(7);
            bool[] days = new bool[] {
                this.Monday,
                this.Tuesday,
                this.Wednesday,
                this.Thursday,
                this.Friday,
                this.Saturday,
                this.Sunday };

            foreach (var day in days)
            {
                sb.Append(day ? "A" : "B");
            }

            return sb.ToString();
        }
		public string MainPeriodText => GetMainPeriodText ();
        private string GetMainPeriodText()
		{
            string start = "Dall'inizio";
            string end = "alla Fine";

			if (this.StartDate != null)
			{
				start = string.Format("Dal {0:dd/MM/yyyy}", this.StartDate);
			}
			if (this.EndDate != null)
			{
				end = string.Format("al {0:dd/MM/yyyy}", this.EndDate);
			}

			return start + " " + end;
	    }

		public void CopyFrom (RunPeriod item)
        {
            this.RunPeriodId = item.RunPeriodId;
            this.StartDate = item.StartDate;
            this.EndDate = item.EndDate;

            this.Monday = item.Monday;
            this.Tuesday = item.Tuesday;
            this.Wednesday = item.Wednesday;
            this.Thursday = item.Thursday;
            this.Friday = item.Friday;
            this.Saturday = item.Saturday;
            this.Sunday = item.Sunday;

            this.Note = item.Note;
            this.Cars = item.Cars;
            this.CarReplacements = item.CarReplacements;
		}

	    public RunPeriod CreateCopy()
        {
            return new RunPeriod
            {
                RunPeriodId = this.RunPeriodId,
                StartDate = this.StartDate,
                EndDate = this.EndDate,

                Monday = this.Monday,
                Tuesday = this.Tuesday,
                Wednesday = this.Wednesday,
                Thursday = this.Thursday,
                Friday = this.Friday,
                Saturday = this.Saturday,
                Sunday = this.Sunday,

                Note = this.Note,
                Cars = this.Cars,
                CarReplacements = this.CarReplacements
            };
        }

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

		private int GetBitmask(params bool[] bits)
		{
			return bits.Select((b, i) => b ? 1 << i : 0).Aggregate((a, b) => a | b);
		}

		private bool[] GetBools(int mask)
		{
			return Enumerable.Range(0, 9).Select(b => (mask & (1 << b)) != 0).ToArray();
		}

	}

}
