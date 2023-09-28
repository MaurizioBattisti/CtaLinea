using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZzSoft.CtaLinea.Dal.Model
{
	public class ExportRunTt
	{
		[ColumnDescription(Header = "")]
		public string RunNote { get; set; }

		[ColumnDescription(Header = "Extra")]
		public bool Extra { get; set; }
		[ColumnDescription(Header = "Nr. Riga Capitolato")]
		public int? ContractRowNumber { get; set; }

		[ColumnDescription(Header = "Linea")]
		public int? LineNumber { get; set; }
		[ColumnDescription(Header = "Nr. Corsa")]
		public string RunNumber { get; set; }

		[ColumnDescription(Header = "Ora Inizio")]
		public TimeSpan? StartTime { get; set; }
		[ColumnDescription(Header = "Ora Fine")]
		public TimeSpan? EndTime { get; set; }

		[ColumnDescription(Header = "Percorso")]
		public string PathsDescr { get; set; }
		[ColumnDescription(Header = "Frequenza Richiesta")]
		public string RequestedFrequency { get; set; }

		[ColumnDescription(Header = "Km corsa/servizio")]
		public double? Km { get; set; }
		[ColumnDescription(Header = "Posti autobus")]
		public int? RequestedCapacity { get; set; }

		[ColumnDescription(Header = "Tod Km")]
		public double? KmTotal { get; set; }

		[ColumnDescription(Header = "Data inizio")]
		public DateTime? StartDate { get; set; }
		[ColumnDescription(Header = "Data Fine")]
		public DateTime? EndDate { get; set; }

		[ColumnDescription(Header = "Ditte Titolari")]
		public string AssociatesDescr { get; set; }
		[ColumnDescription(Header = "MEzzi Titolari")]
		public string PrimaryCarsDescr { get; set; }

		[ColumnDescription(Header = "Ditte riserva")]
		public string SpareAssociatesDescr { get; set; }
		[ColumnDescription(Header = "Mezzi riserva")]
		public string SpareCarsDescr { get; set; }

		[ColumnDescription(Header = "Nr. Giorni")]
		public int? DayCount { get; set; }
		[ColumnDescription(Header = "Id CTA")]
		public int? CtaRunId { get; set; }
	}
}
