using CtaLinea.Model.External;

namespace CtaLinea.Model.Runs
{
    public class RunNode
    {
        public Guid RunNodeId { get; set; }
        public string? CollectionPointId { get; set; }
        public CollectionPointSimple? CollectionPointData { get; set; }

        public TimeSpan Hout { get; set; }
        public int ProgrNumber { get; set; }

        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
    }
}
