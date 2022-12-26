using Radzen;

namespace CtaLineaApp.Application.Model
{
    public class CtaLineaSettings
    {
        public int PageSize { get; set; } = 10;

        // densità delle righe nelle griglie
        public Density Density { get; set; } = Density.Compact;

        #region stili per i mezzi
        public string InactiveCarBgStyle { get; set; } = "rz-background-color-danger-darker";
		public string InactiveCarFgStyle { get; set; } = "rz-color-white";
		public string PrimaryCarBgStyle { get; set; } = "rz-background-color-success-darker";
		public string PrimaryCarFgStyle { get; set; } = "rz-color-white";
		public string SpareCarBgStyle { get; set; } = "rz-background-color-warning-dark";
		public string SpareCarFgStyle { get; set; } = "rz-color-white";
		public string OtherCarBgStyle { get; set; } = string.Empty;
		public string OtherCarFStyle { get; set; } = string.Empty;
		#endregion
	}
}
