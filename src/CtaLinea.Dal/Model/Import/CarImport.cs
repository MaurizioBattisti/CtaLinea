using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model.Import
{
	public class CarImport
	{
		[Index(28)]
		public string BsAssociateId { get; set; }

		[Index(9)]
		public int? NrSittings { get; set; }
		[Index(2)]
		public string RegNumber { get; set; }

		[Index(60)]
		public bool Handicapped { get; set; }
		[Index(64)]
		public bool SChoolBus { get; set; }

		[Index(0)]
		public string BsCarId { get; set; }

		[Index(7)]
		public string ChassisNumber { get; set; }
		
		[Index(12)]
		public string FirstRegistrationDate { get; set; }
		[Index(43)]
		public string DiscontinuationDate { get; set; }

		// per calcoalre primario o meno
		[Index(13)]
		public int? CarUsage { get; set; }
	}
}
