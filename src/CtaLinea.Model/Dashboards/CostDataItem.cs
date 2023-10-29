using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Dashboards
{
	public class CostDataItem
	{
		public string MonthId { get; set; } = string.Empty;
		public decimal DayCost { get; set; }
		public decimal RealCost { get; set; }
		public decimal ContractCost { get; set; }
		public decimal WorkedContractCost { get; set; }
		public decimal NegativeContractCost { get; set; }
		public decimal ExtraCost { get; set; }
	}
}
