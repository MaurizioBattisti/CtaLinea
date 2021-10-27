using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.CtaLinea.Dal.Utility;

namespace ZzSoft.CtaLinea.Dal.Model
{
    public class TtService
    {
        public int ServiceId { get; set; }
    
        [EvaluateChanges(Message ="Numer di Linea")]
        public string LineNumber { get; set; }
        [EvaluateChanges(Message = "Numer di Corsa")]
        public string RunNumber { get; set; }

        [EvaluateChanges(Message = "Descrizione linea")]
        public string LineDescription { get; set; }
        [EvaluateChanges(Message = "Descrizione percorso")]
        public string PathDescription { get; set; }

        public decimal? Km { get; set; }
        [EvaluateChanges(Message = "ID Frequenza")]
        public int? FrequencyId { get; set; }
        [EvaluateChanges(Message = "Descrizione Frequenza")]
        public string FrequencyDescription { get; set; }
        [EvaluateChanges(Message = "ID Linea Tariffaria")]
        public int? TarifLineId { get; set; }
        [EvaluateChanges(Message = "Codice Turno 1")]
        public int? TurnOneId { get; set; }
        [EvaluateChanges(Message = "Codice Turno 2")]
        public int? TurnTwoId { get; set; }
        public DateTime? StartDate { get; set; }
        [EvaluateChanges(Message = "Data Inserimento")]
        public DateTime?  InsertDate { get; set; }
        [EvaluateChanges(Message = "Data Modifica")]
        public DateTime? ModifyDate { get; set; }
        [EvaluateChanges(Message = "Data Ultimo Aggiornamento")]
        public DateTime? LastUpdateDate { get; set; }
        [EvaluateChanges(Message = "ID tipo Servizio")]
        public int? ServiceTypeId { get; set; }
        [EvaluateChanges(Message = "Località Partenza")]
        public string StartingLocation { get; set; }
        [EvaluateChanges(Message = "Località Arrivo")]
        public string EndingLocation { get; set; }
        public string Note { get; set; }
        [EvaluateChanges(Message = "Ora Inizio")]
        public DateTime? StartHour { get; set; }
        [EvaluateChanges(Message = "Ora Fine")]
        public DateTime? EndHour { get; set; }
        [EvaluateChanges(Message = "Descrizione Frequenza Prospetto")]
        public string TimingDescr { get; set; }
        [EvaluateChanges(Message = "Nr Posti richiesti da Capitolato")]
        public int? RequestedSittings { get; set; }
        [EvaluateChanges(Message = "Riferimento Capitolato")]
        public string SpecReference { get; set; }
        [EvaluateChanges(Message = "Tipo Corsa")]
        public string RunType { get; set; }
        [EvaluateChanges(Message = "Descrizione Tipo Servizio")]
        public string ServiceTypeDescr { get; set; }
        [EvaluateChanges(Message = "Codice Variante")]
        public int? VariantId { get; set; }
        [EvaluateChanges(Message = "Corsa su Tabella Orario")]
        public bool RunInHoursBook { get; set; }
        [EvaluateChanges(Message = "Ditta")]
        public string Company { get; set; }
        [EvaluateChanges(Message = "Ditta 1")]
        public string Company1 { get; set; }
        [EvaluateChanges(Message = "Ditta 2")]
        public string Company2 { get; set; }
        [EvaluateChanges(Message = "Ditta 3")]
        public string Company3 { get; set; }
        [EvaluateChanges(Message = "Tabella Destinazione")]
        public string DestinationTable { get; set; }
    }
}
