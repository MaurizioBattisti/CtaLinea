using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Utilities
{
	public class RunPlanningItem
	{
		public Guid RunId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }

		public Guid RunVariationId { get; set; }
		public Guid? RunPeriodId { get; set; }

		public string VariationDescr { get; set; } = string.Empty;
		public string PeriodDescr { get; set; } = string.Empty;

		public string Text => this.VariationDescr + " " + this.PeriodDescr; 
    }
}
