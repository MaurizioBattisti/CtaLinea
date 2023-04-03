using CtaLinea.Model.Base;

namespace CtaLinea.Model.Runs
{
    public class RunSuspension
    {
        public Guid RunSuspensionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SuspensionTypeId { get; set; }
        public SuspensionType? SuspensionTypeData { get; set; }
        public string? SuspensionNote { get; set; }


        public void CopyFrom (RunSuspension item)
        {
			this.RunSuspensionId = item.RunSuspensionId;
			this.StartDate = item.StartDate;
			this.EndDate = item.EndDate;
			this.SuspensionTypeId = item.SuspensionTypeId;
			this.SuspensionTypeData = item.SuspensionTypeData;
			this.SuspensionNote = item.SuspensionNote;
		}

		public RunSuspension GetCopy ()
        {
            return new RunSuspension()
            {
				RunSuspensionId = this.RunSuspensionId,
				StartDate = this.StartDate,
				EndDate = this.EndDate,
				SuspensionTypeId = this.SuspensionTypeId,
				SuspensionTypeData = this.SuspensionTypeData,
				SuspensionNote = this.SuspensionNote
		    };  

		}
	}
}
