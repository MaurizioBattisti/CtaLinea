using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.Filters
{
    public class RunAdvancedFilters
    {
        // dati filtri generali
        public int? ContractId { get; set; }
        public DateTime? StartPeriod { get; set; }
        public DateTime? EndPeriod { get; set; }

        public Guid? AssociateId { get; set; }
        public Guid? CarId { get; set; }
        public int? MinSittings { get; set; }
        public int? MaxSittings { get; set; }
        public int? LineNumber { get; set; }
        public string? RunNumber { get; set; }
        public string? Node { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime?  EndDate { get; set; }

        public TimeSpan? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
        public string? Frequency { get; set; }
        public IEnumerable<int>? CalendarIds { get; set; }
        public IEnumerable<int>? WeekDays { get; set; }
        public int? InContract { get; set; }
        public int? ActiveRun { get; set; }
        public DateTime? DateRef { get; set; }
        public IEnumerable<int>? TabIds { get; set; }

        public string? CollectionPointId { get; set; } 
        public int? ForfaitId { get; set; }


        #region Azzera tutti i filtri
        public void ClearAll ()
        {
            this.AssociateId = null;
            this.CarId = null;
            this.MinSittings = null;
            this.MaxSittings = null;
            this.LineNumber = null;
            this.RunNumber = null;
            this.Node = null;
            this.StartDate = null;
            this.EndDate = null;

            this.StartTime = null;
            this.EndTime = null;
            this.Frequency = null;
            this.CalendarIds = null;
            this.WeekDays = null;
            this.InContract = null;
            this.ActiveRun = null;
            this.DateRef = null;
            this.TabIds = null;

            this.CollectionPointId = null;
            this.ForfaitId = null;
        }
        #endregion

        #region assegna i valori a null dove non hanno impatto
        public void CleanInconsistency ()
        {
            if (this.AssociateId == Guid.Empty) this.AssociateId = null;
            if (this.CarId == Guid.Empty) this.CarId = null;
            if (string.IsNullOrWhiteSpace(this.Node)) this.Node = null;
            if (string.IsNullOrWhiteSpace(this.CollectionPointId)) this.CollectionPointId = null;

            if (string.IsNullOrWhiteSpace(this.Frequency)) this.Frequency = null;
            
            if (this.CalendarIds?.Count() == 0)this.CalendarIds = null;
            if (this.WeekDays?.Count() == 0) this.WeekDays = null;
            
            if (this.ActiveRun == null)
            {
                this.DateRef = null;
            }
            if (this.TabIds?.Count() == 0) this.TabIds = null;
        }
        #endregion

        public bool HasImpact ()
        {
            this.CleanInconsistency();
            bool hasImpact = !(
                this.AssociateId == null
                && this.CarId == null
                && this.MinSittings == null
                && this.MaxSittings == null
                && this.LineNumber == null
                && this.RunNumber == null
                && this.Node == null
                && this.StartDate == null
                && this.EndDate == null
                && this.StartTime == null
                && this.EndTime == null
                && this.Frequency == null
                && this.CalendarIds == null
                && this.WeekDays == null
                && this.InContract == null
                && this.ActiveRun == null
                && this.DateRef == null
                && this.TabIds == null

                && this.CollectionPointId == null
                && this.ForfaitId == null
                );

            return hasImpact;
        }
    }
}
