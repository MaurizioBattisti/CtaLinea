using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Import
{
	public class DriverImport
	{
		[Index(29)]
		public string BsAssociateId { get; set; }

		[Index(52)]
		public string BsDriverId { get; set; }

		[Index(0)]
		public string LastName { get; set; }
		[Index(1)]
		public string FirstName { get; set; }

		[Index(11)]
		public string LicenseNumber { get; set; }
		[Index(12)]
		public string LicenceCategory { get; set; }

		[Index(28)]
		public string DismissionDate { get; set; }

		[Index(10)]
		public bool Active { get; set; }
		
		[Index(66)]
		public Guid ? DriverId { get; set; }
	}
}
