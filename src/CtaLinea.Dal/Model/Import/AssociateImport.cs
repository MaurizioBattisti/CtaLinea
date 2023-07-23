using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Import
{
	public class AssociateImport
	{
		[Index(1)]
		public string Description { get; set; }
		[Index(16)]
		public string BsSupplierCode { get; set; }
		/*
		public string BsCustomerCode { get; set; }
		public bool Active { get; set; }
		*/

		[Index(42)]
		public string V2AssociateId { get; set; }

		[Index(21)]
		public string Email { get; set; }
	}
}
