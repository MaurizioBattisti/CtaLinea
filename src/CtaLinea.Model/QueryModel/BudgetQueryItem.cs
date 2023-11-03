using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
	[SqlAlias("b")]
	public class BudgetQueryItem
	{
		[SqlField("BudgetId")]
		public int Id {  get; set; }
		[SqlField(FullText = true, SortPosition = 0)]
		public string BudgetName { get; set; } = string.Empty;
		public string BudgetType { get; set; } = string.Empty;
		[SqlField(SortPosition = 1)]
		public DateTime BudgetDate { get; set; }

		public int? ContractId { get; set; }
		[SqlField(FullText = true)]
		public string ContractDescr { get; set; } = string.Empty;

		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }

		public Guid? AssociateId { get; set; }
		[SqlField(FullText = true)]
		public string AssociateDescr { get; set; } = string.Empty;
		public Guid? CarId { get; set; }
		[SqlField(FullText = true)]
		public string CarDescr { get; set; } = string.Empty;
		public Guid? RunId { get; set; }
		[SqlField(FullText = true)]
		public string CtaRunId { get; set; } = string.Empty;

		public bool OutOfPEriod { get; set; }
		public bool Suspended { get; set; }
		public bool RplacedCars { get; set; } = true;
	}
}
