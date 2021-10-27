using System;
using System.Collections.Generic;
using System.Text;
using ZzSoft.CtaLinea.Dal.Utility;

namespace ZzSoft.CtaLinea.Dal.Model
{
    public class TtServiceNode
    {
        [EvaluateChanges(Ignore = true)]
        public int NodeId { get; set; }
        public int ServiceId { get; set; }
        [EvaluateChanges(Message = "Codice Nodo / Id Punto di Raccolta")]
        public string CollectionPointId { get; set; }
        [EvaluateChanges(Message = "Descrizione nodo")]
        public string Description { get; set; }
        [EvaluateChanges(Message = "Numero progressivo nodo")]
        public int ProgrNumber { get; set; }
        [EvaluateChanges(Message = "Orario")]
        public DateTime? Hour { get; set; }
        [EvaluateChanges(Message = "Minuti di fermata")]
        public int? MinutesStop { get; set; }
        [EvaluateChanges(Message = "Orario ARrivo")]
        public DateTime? ArrivedAtHour { get; set; }
        [EvaluateChanges(Message = "Longitudine")]
        public decimal? Longitude { get; set; }
        [EvaluateChanges(Message = "Latitudine")]
        public decimal? Latitude { get; set; }
    }
}
