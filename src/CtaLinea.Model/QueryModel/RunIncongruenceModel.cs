using CtaLinea.Model.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using ZzSoft.QueryHelper;

namespace CtaLinea.Model.QueryModel
{
    public class RunIncongruenceModel
    {
        [ColumnDescription(Header = "Id")]
        public Guid RunId { get; set; }
        [ColumnDescription(Header = "Id CTA")]
        public int CtaRunId { get; set; }

        [ColumnDescription(Header = "Nome corsa")]
        public string? RunName { get; set; }

        [ColumnDescription(Header = "Id Appalto")]
        public int? ContractId { get; set; }

        [ColumnDescription(Header = "Extra")]
        public bool Extra { get; set; }
        [ColumnDescription(Header = "Riga Appalto")]
        public int? ContractRowNumber { get; set; }

        [ColumnDescription(Header = "Data Inizio")]
        public DateTime? StartDate { get; set; }
        [ColumnDescription(Header = "Data Fine")]
        public DateTime? EndDate { get; set; }

        [ColumnDescription(Header = "GG. Richiesti")]
        public int? RequestedDays { get; set; }
        [ColumnDescription(Header = "Note Corsa")]
        public string? RunNote { get; set; }

        // Descrizioni
        [ColumnDescription(Header = "Ditte")]
        public string? AssociatesDescr { get; set; }
        [ColumnDescription(Header = "Titolari")]
        public string? PrimaryCarsDescr { get; set; }
        [ColumnDescription(Header = "Scorte")]
        public string? SpareCarsDescr { get; set; }
        [ColumnDescription(Header = "Calendari")]
        public string? CalendarsDescr { get; set; }
        [ColumnDescription(Header = "Percorsi")]
        public string? PathsDescr { get; set; }

        [ColumnDescription(Header = "Id Variante", Ignore = true)]
        public Guid RunVariationId { get; set; }
        [ColumnDescription(Header = "Data Inizio Variante")]
        public DateTime? VariationStartDate { get; set; }
        [ColumnDescription(Header = "Nr. Linea")]
        public int? LineNumber { get; set; }
        [ColumnDescription(Header = "Nr. Corsa")]
        public string? RunNumber { get; set; }

        [ColumnDescription(Header = "Ora Inizio")]
        public TimeSpan? StartTime { get; set; }
        [ColumnDescription(Header = "Ora Fine")]
        public TimeSpan? EndTime { get; set; }

        [ColumnDescription(Header = "Lunedì")]
        public bool Monday { get; set; }
        [ColumnDescription(Header = "Martedì")]
        public bool Tuesday { get; set; }
        [ColumnDescription(Header = "Mercoledì")]
        public bool Wednesday { get; set; }
        [ColumnDescription(Header = "Giovedì")]
        public bool Thursday { get; set; }
        [ColumnDescription(Header = "Venerdì")]
        public bool Friday { get; set; }
        [ColumnDescription(Header = "Sabato")]
        public bool Saturday { get; set; }
        [ColumnDescription(Header = "Domenica")]
        public bool Sunday { get; set; }

        [ColumnDescription(Header = "Percorso")]
        public string? Path { get; set; }
        [ColumnDescription(Header = "Frequenza Richiesta")]
        public string? RequestedFrequency { get; set; }

        [ColumnDescription(Header = "Km")]
        public double? Km { get; set; }
        [ColumnDescription(Header = "Capienza Richiesta")]
        public int? RequestedCapacity { get; set; }
        [ColumnDescription(Header = "Note Variante")]
        public string? VariationNote { get; set; }


        [ColumnDescription(Header = "Appalto")]
        public string? ContractDescription { get; set; }
        [ColumnDescription(Header = "Inizio Appalto")]
        public DateTime? ContractStart { get; set; }
        [ColumnDescription(Header = "Fine Appalto")]
        public DateTime? ctrEndDAte { get; set; }

        [ColumnDescription(Header = "Nr. Varianti")]
        public int VariationCount { get; set; }

        [ColumnDescription(Header = "Etichetta")]
        public string? TagName { get; set; }
        [ColumnDescription(Ignore = true)]
        public string? BgColor { get; set; }
        [ColumnDescription(Ignore = true)]
        public string? Color { get; set; }

        [ColumnDescription(Ignore = true)]
        public int RunStatus { get; set; }
        [ColumnDescription(Header = "Status")]
        public string? RunStatusDesvr { get; set; }

        [ColumnDescription(Header = "Id Forfait")]
        public int? ForfaitId { get; set; }
        [ColumnDescription(Header = "Forfait")]
        public string? ForfaitName { get; set; }
        [ColumnDescription(Header = "Tipo Forfait")]
        public string? ForfaitType { get; set; }

        [SqlAlias("n")]
        [ColumnDescription(Header = "Coincidenza")]
        public string? CoincidenceState { get; set; }

        [ColumnDescription(Header = "Descrizione Incongreunze")]
        public string? IncongreunceDescr { get; set; }


		[ColumnDescription(Header = "Bloccata fino al", Ignore = false)]
		public DateTime? LockedDate { get; set; }
		[ColumnDescription(Header = "Note di Blocco", Ignore = false)]
		public string? LockedNote { get; set; }

		public void CopyFrom(RunIncongruenceModel source)
        {
            RunId = source.RunId;
            CtaRunId = source.CtaRunId;
            ContractId = source.ContractId;

            Extra = source.Extra;
            ContractRowNumber = source.ContractRowNumber;
            StartDate = source.StartDate;
            EndDate = source.EndDate;

            RequestedDays = source.RequestedDays;
            RunNote = source.RunNote;

            AssociatesDescr = source.AssociatesDescr;
            PrimaryCarsDescr = source.PrimaryCarsDescr;
            SpareCarsDescr = source.SpareCarsDescr;
            CalendarsDescr = source.CalendarsDescr;
            PathsDescr = source.PathsDescr;

            RunVariationId = source.RunVariationId;
            VariationStartDate = source.VariationStartDate;
            LineNumber = source.LineNumber;
            RunNumber = source.RunNumber;
            StartTime = source.StartTime;
            EndTime = source.EndTime;

            Monday = source.Monday;
            Tuesday = source.Tuesday;
            Wednesday = source.Wednesday;
            Thursday = source.Thursday;
            Friday = source.Friday;
            Saturday = source.Saturday;
            Sunday = source.Sunday;

            Path = source.Path;
            RequestedFrequency = source.RequestedFrequency;

            Km = source.Km;
            RequestedCapacity = source.RequestedCapacity;
            VariationNote = source.VariationNote;

            ContractDescription = source.ContractDescription;
            ContractStart = source.ContractStart;
            ctrEndDAte = source.ctrEndDAte;

            VariationCount = source.VariationCount;

            TagName = source.TagName;
            BgColor = source.BgColor;
            Color = source.Color;
            RunStatus = source.RunStatus;

            ForfaitId = source.ForfaitId;
            ForfaitName = source.ForfaitName;
            ForfaitType = source.ForfaitType;

            CoincidenceState = source.CoincidenceState;

            IncongreunceDescr = source.IncongreunceDescr;

            LockedDate = source.LockedDate;
            LockedNote = source.LockedNote;
		}
    }
}
