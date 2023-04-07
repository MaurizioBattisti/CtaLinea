using CtaLinea.Model.External;

namespace CtaLinea.Model.Runs
{
    public class RunNode
    {
        public Guid RunNodeId { get; set; }
        public string? CollectionPointId { get; set; }
        public CollectionPointSimple? CollectionPointData { get; set; }

        public TimeSpan Hour { get; set; }
        public int ProgrNumber { get; set; }

        public void copyFrom (RunNode item)
        {
			this.RunNodeId = item.RunNodeId;
			this.CollectionPointId = item.CollectionPointId;
			this.CollectionPointData = item.CollectionPointData; ;
			this.Hour = item.Hour;
			this.ProgrNumber = item.ProgrNumber;
		}

		public RunNode GetClone ()
        {
            return new RunNode()
            {
				RunNodeId = this.RunNodeId,
				CollectionPointId = this.CollectionPointId,
				CollectionPointData = this.CollectionPointData,
				Hour = this.Hour,
				ProgrNumber = this.ProgrNumber
        	};
		}
	}
}
