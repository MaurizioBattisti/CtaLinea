using CtaLinea.Model.Helpers;
using CtaLinea.Model.ScheduledTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ModelServices
{
    public class SchedulerTaskChecker 
        : ISchedulerTaskChecker
    {
        public async Task<CheckResult> CheckASync(
            ScheduledTaskItem item)
        {
            var errors = new List<CheckResultItem>();
            var warnings = new List<CheckResultItem>();
            var informations = new List<CheckResultItem>();
            var result = new RunCheckResult()
            {
                Status = CheckStatus.Success,
                Errors = errors,
                Warnings = warnings,
                Informations = informations
            };

            if (item.Id == 1 
                && item.ActivityId != "reloadscheduler")
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "l'attività di ricarica dello scheduler deve essere mantenuta sull'id speciale '1'.",
                    Title = string.Empty,
                });
            }
            if (item.EndTime != TimeSpan.Zero
                && item.EndTime != TimeSpan.Zero)
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "Deve essere indicato un intervallo se si indica una finestra operativa per l'attività",
                    Title = string.Empty,
                });
            }
            if (item.StartTime < TimeSpan.Zero || item.StartTime > TimeSpan.FromHours(24))
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "L'orario di inizio deve essere compreso nel range 0:00 <= valore <  24:00",
                    Title = string.Empty,
                });
            }
            if (item.EndTime < TimeSpan.Zero || item.EndTime > TimeSpan.FromHours(24))
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "L'orario di fine deve essere compreso nel range 0:00 <= valore <  24:00",
                    Title = string.Empty,
                });
            }
            if (item.Interval < TimeSpan.Zero || item.Interval > TimeSpan.FromHours(24))
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "L'intervallo deve essere compreso nel range 0:00 <= valore <  24:00",
                    Title = string.Empty,
                });
            }
            if (item.EndTime != TimeSpan.Zero
                && item.EndTime < item.StartTime)
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "L'orario di fine non può precedere quello di inizio",
                    Title = string.Empty,
                });
            }
            if (item.Frequency == ScheduleFrequency.Daily
                && item.RrequencyMask != 0)
            {
                warnings.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "La maschera di frequenza non può essere impostat se il tipo di frequenza è giornaliero",
                    Title = string.Empty,
                });
            }
            if (item.Frequency == ScheduleFrequency.Monthly
                && (item.RrequencyMask > 31
                    || item.RrequencyMask < 1
                ))
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "per le attività mensili la maschera di frequenza deve essere comrpesa tra 1 e 31 occhio nell'uso dei valori 29 , 30 e 31 in quanto non tutti i mesi hanno 29 giorni",
                    Title = string.Empty,
                });
            }
            if (item.Timeout < 0
                || item.Timeout > 3600)
            {
                errors.Add(new CheckResultItem()
                {
                    Category = string.Empty,
                    Description = "Il timeout deve essere compreso tra 0 e 3600 secondi (0 indica di usare un valore di default)",
                    Title = string.Empty,
                });
            }

            // controlla se nella lsita di dettaglaio ci sono erorri
            if (errors.Count > 0) result.Status = CheckStatus.Failed;
            else if (warnings.Count > 0) result.Status = CheckStatus.Warning;
            else if (informations.Count > 0) result.Status = CheckStatus.Information;

            if (errors.Count > 0)
            {
                result.Title = "Errori";
                result.Description = "Errori nella definizione dell'appalto";
            }

            return await Task.FromResult(result);
        }
    }
}
