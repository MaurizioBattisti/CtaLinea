using CtaLinea.Model.Base;

namespace CtaLinea.Model.Runs
{
    public class RunSuspension
    {
        public Guid RunSuspensionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SuspensionTypeId { get; set; }
        public SuspensionType? SuspensionTypeData { get; set; }
        public string? SuspensionNote { get; set; }
    }
}
