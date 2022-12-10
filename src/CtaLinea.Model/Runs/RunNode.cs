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
