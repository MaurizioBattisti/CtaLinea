using CtaLinea.Model.External;
using CtaLinea.Model.Helpers;
using CtaLinea.Model.Runs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ModelServices
{
    public class RunChecker 
        : IRunChecker
    {
        public async Task<RunItem> CleanGraphAsync (RunItem run)
        {
            if (run.SubPeriods != null)
            {
                foreach (var period in run.SubPeriods)
                {
                    // recupera i runPeriodId di tutte le sostituzioni
                    if (period.CarReplacements != null)
                    {
                        if (period.Cars != null)
                        {
                            var ids = period.CarReplacements.Where(r => r.ReplacedPEriodCarIds != null).SelectMany(r => r.ReplacedPEriodCarIds);
                            if (ids != null)
                            {
                                var toDel = (from c in period.Cars
                                             where c.RunCarType == CarTypeEnum.Replacement
                                                && ids.Contains(c.RunCarId) == false
                                             select c).ToList();
                                foreach (var c in toDel)
                                {
                                    period.Cars.Remove(c);
                                }
    
                            }
                        }
                    }

                    // gestisce i costi dei mezzi
                    if (period.Cars != null)
                    {
                        foreach (var car in period.Cars)
                        {
                            // se il mezzo è una scorta
                            if (car.RunCarType == CarTypeEnum.Spare)
                            {
                                // i costi devono essere tolti
                                car.CarCosts = new List<RunCarCost>();
                            }
                        }
                    }
                }
            }

            return await Task.FromResult(run);
        }

        public async Task<RunCheckResult> CheckRunAsync(
            RunItem run)
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

            // controlla tutti i dati
            if (run.ContractData != null
                && run.StartDate != null)
            {
                if (run.StartDate < run.ContractData.StartDate
                    || run.StartDate > run.ContractData.EndDate)
                {
                    // data di inizio della corsa furo periodo
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Run,
                            Title = "Data inizio non valida",
                            Description = "la data di inizio della corsa è al di fuori dell'intervallo valido per l'appalto.",
                            Id = run.RunId
                        });
                }
            }
            if (run.ContractData != null
                && run.EndDate != null)
            {
                if (run.EndDate < run.ContractData.StartDate
                    || run.StartDate > run.ContractData.EndDate)
                {
                    // data di inizio della corsa furo periodo
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Run,
                            Title = "Data Fine non valida",
                            Description = "la data di fine della corsa è al di fuori dell'intervallo valido per l'appalto.",
                            Id = run.RunId
                        });
                }
            }
            if (run.StartDate != null 
                && run.EndDate != null
                && run.StartDate > run.EndDate)
            {
                // le date non sono congruenti tra di loro
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Run,
                        Title = "Date incoerenti",
                        Description = "La data di fine della corsa precede quella di inizio",
                        Id = run.RunId
                    });
            }

            if (run.Extra == false
                && run.ContractRowNumber == null)
            {
                warnings.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Run,
                        Title = "Rifertimento riga di appalto",
                        Description = "Non è stato indicato il riferimento della riga di appalto",
                        Id = run.RunId
                    });
            }

            // controlla le varianti
            await this.CheckAllVariantsAsync(run, errors, warnings, informations);

            // contorlla i periodio
            if (run.SubPeriods == null
                || run.SubPeriods.Count == 0)
            {
                warnings.Add(
                    new CheckResultItem ()
                    {
                        Category = RunCheckResult.Category_Run,
                        Title = "Nessun periodo di operatività",
                        Description = "Non è stato indicato nessun periodo di operatività dei mezzi",
                        Id = run.RunId
                    });
            }
            else
            {
                foreach (var period in run.SubPeriods)
                {
                    await this.CheckPeriodAsync(run, period, errors, warnings, informations);
                }
            }

            // controlla le sospensioni
            if (run.Suspensions != null
                && run.Suspensions.Count > 0)
            {
                foreach (var suspension in run.Suspensions)
                {
                    await this.CheckSuspensionAsync(run, suspension, errors, warnings, informations);
                }
            }
            // controlla se nella lsita di dettaglaio ci sono erorri
            if (errors.Count > 0) result.Status = CheckStatus.Failed;
            else if (warnings.Count > 0) result.Status = CheckStatus.Warning;
            else if (informations.Count > 0) result.Status = CheckStatus.Information;

            return await Task.FromResult(result);
        }

        #region utility
        private T? Coalesce<T>(params T?[] items)
        {
            T? value = default;
            foreach (var item in items)
            {
                if (item != null)
                {
                    value = item;
                    break;
                }
            }
            return value;
        }
        private DateTime GetMinDate (
            RunItem run,
            RunPeriod? period = null
            )
        {
            var dt = this.Coalesce(
                period?.StartDate,
                run.StartDate,
                run.ContractData?.StartDate,
                DateTime.MinValue);
            if (dt == null) return DateTime.MinValue;
            return dt.Value;
        }
        private DateTime GetMaxDate(
            RunItem run,
            RunPeriod? period = null
            )
        {
            var dt = this.Coalesce(
                period?.EndDate,
                run.EndDate,
                run.ContractData?.EndDate,
                DateTime.MaxValue);
            if (dt == null) return DateTime.MaxValue;
            return dt.Value;
        }
        #endregion

        #region variations
        private async Task CheckAllVariantsAsync (
            RunItem run,
            IList<CheckResultItem> errors,
            IList<CheckResultItem>  warnings,
            IList<CheckResultItem> informations
            )
        {
            // contorlla ch ci sia almeno una variante
            if (run.Variations == null
                || run.Variations.Count == 0)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Run,
                        Title = "Nessuna Variante",
                        Description = "Non è stata indicata nessuna variante",
                        Id = run.RunId
                    });
            }
            else
            {
                var FirstVarCount = run.Variations.Where(v => v.StartDate == null).Count();
                if (FirstVarCount == 0)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Run,
                            Title = "Nessuna variante principale",
                            Description = "Non è stata indicata nessuna variante principale, la variante principale  è quella senza la data di inizio.",
                            Id = run.RunId
                        });
                }
                else if (FirstVarCount > 1)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Run,
                            Title = "Troppe varianti principali",
                            Description = "Non è possibile indicare più di una variante principale.",
                            Id = run.RunId
                        });
                }

                // esegue un controllo per ogni variante
                foreach (var v in  run.Variations)
                {
                    await this.CheckVariantAsync(run,
                        v,
                        errors, warnings, informations);
                }
            }

            await Task.CompletedTask;
        }

        private async Task CheckVariantAsync(
            RunItem run,
            RunVariation variation,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (variation.StartDate != null)
            {
                // controlal che la data di inizio della variante sia nel range
                if (variation.StartDate < this.GetMinDate(run) 
                    || variation.StartDate > this.GetMaxDate(run))
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_RunVariation,
                            Title = "Data inizio errata",
                            Description = "La data di inizio della variante è fuori dal periodo di validità della corsa.",
                            Id = variation.RunVariationId
                        });
                }
            }
            if (string.IsNullOrWhiteSpace(variation.Path) == true)
            {
                warnings.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_RunVariation,
                        Title = "Percorso vuoto",
                        Description = "E' stato indicato un percorso vuoto.",
                        Id = variation.RunVariationId
                    });
            }
            if (variation.CalendarId == null)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_RunVariation,
                        Title = "Calendario mancante",
                        Description = "Deve essere indicato il calendario",
                        Id = variation.RunVariationId
                    });
            }
            // controlla che sia stato indicato almeno un giorno della settimana
            if (variation.Monday ==false
                && variation.Tuesday == false
                && variation.Wednesday == false
                && variation.Thursday == false
                && variation.Friday == false
                && variation.Saturday == false
                && variation.Sunday == false)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_RunVariation,
                        Title = "Giorno settimana mancante",
                        Description = "Deve essere specificato almeno un giorno della settimana",
                        Id = variation.RunVariationId
                    });
            }
            if (variation.Km == null 
                || variation.Km.Value <= 0)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_RunVariation,
                        Title = "KM non indicati",
                        Description = "I chilometri non sono stati indicati oppure sono zero o inferiori, cosa non possibile.",
                        Id = variation.RunVariationId
                    });
            }

            // controlla i nodi della variante
            if (variation.Nodes == null
                || variation.Nodes.Count () == 0)
            {
                warnings.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_RunVariation,
                        Title = "Nessun nodo",
                        Description = "Non sono stati indicati nodi per la variante della corsa",
                        Id = variation.RunVariationId
                    });
            }
            else
            {
                foreach (var node in variation.Nodes)
                {
                    await this.CheckNodeAsync(run, variation, node,
                        errors, warnings, informations);
                }
            }

            await Task.CompletedTask;
        }

        #region nodi
        private async Task CheckNodeAsync(
            RunItem run,
            RunVariation variation,
            RunNode node,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (string.IsNullOrWhiteSpace( node.CollectionPointId) == true)
            {
                warnings.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Node,
                        Title = "Punto di raccolta mancante",
                        Description = "Non è stato indicato il punto di raccolta",
                        Id = node.RunNodeId
                    });
            }

            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region subperiods
        private async Task CheckPeriodAsync (
            RunItem run,
            RunPeriod period,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            // controlla i dati generali del periodo
            #region dati generali periodo
            if (period.StartDate != null)
            {
                if (period.StartDate <= this.GetMinDate(run)
                    || period.StartDate >= this.GetMaxDate(run)
                    )
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Period,
                            Title = "Data inizio periodo fuori limiti",
                            Description = "La data di inizio del periodo è fuori dai limiti della corsa",
                            Id = period.RunPeriodId
                        });
                }
            }
            if (period.StartDate != null)
            {
                if (period.EndDate <= this.GetMinDate(run)
                    || period.EndDate >= this.GetMaxDate(run)
                    )
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Period,
                            Title = "Data fine periodo fuori limiti",
                            Description = "La data di fine del periodo è fuori dai limiti della corsa",
                            Id = period.RunPeriodId
                        });
                }
            }
            if (period.StartDate != null
                && period.EndDate != null
                && period.StartDate > period.EndDate)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Period,
                        Title = "Date del periodo incongruenti",
                        Description = "La data di inizio del periodo è successiva alla data di fine",
                        Id = period.RunPeriodId
                    });
            }
            // controlla che sia stato indicato almeno un giorno della settimana
            if (period.Monday == false
                && period.Tuesday == false
                && period.Wednesday == false
                && period.Thursday == false
                && period.Friday == false
                && period.Saturday == false
                && period.Sunday == false)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Period,
                        Title = "Giorno settimana mancante",
                        Description = "Deve essere specificato almeno un giorno della settimana",
                        Id = period.RunPeriodId
                    });
            }
            #endregion

            // contorlla i mezzi
            if (period.Cars == null
                || period.Cars.Count == 0)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Period,
                        Title = "Nessun mezzo",
                        Description = "Deve essere indicato almeno un mezzo sul periodo di operatività",
                        Id = period.RunPeriodId
                    });
            }
            else
            {
                // controla che tra i titolari e le scorte non ci siano due volte lo stesso mezzo
                var periodCArs = period.Cars.Where(c => c.RunCarType == CarTypeEnum.Primary || c.RunCarType == CarTypeEnum.Spare).Select(c => c.CarId);
                if (periodCArs.Count() != periodCArs.Distinct().Count ())
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Period,
                            Title = "Mezzi doppi",
                            Description = "E' stato indicato più volte lo stesso mezzo tra i titolari e  le scorte",
                            Id = period.RunPeriodId
                        });
                }

                foreach (var periodCar in period.Cars)
                {
                    await this.CheckPeriodCarAsync(run, period, periodCar, errors, warnings, informations);
                }

                // controlalc he ci sia almeno un titolare
                var primary = period.Cars.Where(c => c.RunCarType == CarTypeEnum.Primary).FirstOrDefault();
                if (primary == null)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Period,
                            Title = "Nessun Titolare",
                            Description = "Deve essere indicato almeno un mezzo titolare.",
                            Id = period.RunPeriodId
                        });
                }

                // contorlla che ci sia almeno una scorta
                var spare = period.Cars.Where(c => c.RunCarType == CarTypeEnum.Spare).FirstOrDefault();
                if (spare == null)
                {
                    warnings.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Period,
                            Title = "Nessuna Scorta",
                            Description = "Non è stato indicato neanche un mezzo di scorta",
                            Id = period.RunPeriodId
                        });
                }
            }

            if (period.CarReplacements != null
                && period.CarReplacements.Count > 0)
            {
                // esegue il controllo delle sostituzioni
                foreach (var repl in period.CarReplacements)
                {
                    await this.CheckPeriodCarReplacementAsync (run, period, repl, errors, warnings, informations);
                }
            }

            await Task.CompletedTask;
        }

        #region mezzi
        private async Task CheckPeriodCarAsync(
            RunItem run,
            RunPeriod period,
            RunPeriodCar car,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (car.RunCarType == CarTypeEnum.Primary
                || car.RunCarType == CarTypeEnum.Replacement)
            {
                if (car.CarCosts == null
                    || car.CarCosts.Count == 0)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Car,
                            Title = "Costi mezzi mancanti",
                            Description = "Deve essere indicato almeno un costo per i mezzi titolari o le sostituzioni",
                            Id = car.RunCarId
                        });
                }
                else
                {
                    // contorlla che ci sia il costo principale quello con startdate nulla
                    var mainCost = car.CarCosts.Where(c => c.StartDate == null).FirstOrDefault();
                    if (mainCost == null)
                    {
                        errors.Add(
                            new CheckResultItem()
                            {
                                Category = RunCheckResult.Category_Car,
                                Title = "Costo iniziale mancante",
                                Description = "Deve essere indicato il costo principale, quello con la data di inizio vuota",
                                Id = car.RunCarId
                            });
                    }

                    // controlla i costi solo se il mezzo è un titolare o una sostituzione
                    foreach (var cost in car.CarCosts)
                    {
                        await this.CheckPeriodCarCostAsync(run, period, car, cost, errors, warnings, informations);
                    }
                }
            }
            else
            {
                // avverte se ci sono dei costi che non dovrebbero eseerci
                if (car.CarCosts != null
                    && car.CarCosts.Count > 0)
                {
                    informations.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_Car,
                            Title = "Costi inutili",
                            Description = "Sono presenti dei costi su un mezzo scorta che non verranno mai considerati",
                            Id = car.RunCarId
                        });
                }
            }

            await Task.CompletedTask;
        }
        #region costi dei mezzi
        private async Task CheckPeriodCarCostAsync(
            RunItem run,
            RunPeriod period,
            RunPeriodCar car,
            RunCarCost cost,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (cost.StartDate != null
                &&  (cost.StartDate < this.GetMinDate(run, period)
                    || cost.StartDate > this.GetMaxDate (run, period))
                )
            {
                informations.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Car,
                        Title = "Costi fuori intervallo",
                        Description = "Sono presenti dei costi che iniziano al di fuori del periodo di validità dell'appalto, della corsa o del sottoperiodo  d operatiività del mezzo",
                        Id = car.RunCarId
                    });
            }

            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region sostituzioni dei mezzi
        private async Task CheckPeriodCarReplacementAsync(
            RunItem run,
            RunPeriod period,
            CarReplacement carReplacement,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (period.Cars != null)
            {
                if (carReplacement.StartDate != null
                    && (carReplacement.StartDate < this.GetMinDate(run, period)
                        || carReplacement.StartDate > this.GetMaxDate(run, period))
                    )
                {
                    warnings.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_CarReplacement,
                            Title = "inizio sostituzione furoi internvallo",
                            Description = "La data di inizio della sostituzione è al di fuori dell'intervallo di validità dell'appalto, della corsa  o del periodo di operatività del mezzo.",
                            Id = carReplacement.CarReplacementId
                        });
                }
                if (carReplacement.EndDate != null
                    && (carReplacement.EndDate < this.GetMinDate(run, period)
                        || carReplacement.EndDate > this.GetMaxDate(run, period))
                    )
                {
                    warnings.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_CarReplacement,
                            Title = "Fine sostituzione furoi internvallo",
                            Description = "La data di fine della sostituzione è al di fuori dell'intervallo di validità dell'appalto, della corsa  o del periodo di operatività del mezzo.",
                            Id = carReplacement.CarReplacementId
                        });
                }
                if (carReplacement.StartDate != null
                    && carReplacement.EndDate != null
                    && carReplacement.StartDate > carReplacement.EndDate)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_CarReplacement,
                            Title = "Date della sostituzione incongruenti",
                            Description = "La data di inizio della sostituzione è successiva  a quella di fine",
                            Id = carReplacement.CarReplacementId
                        });
                }

                // controlla che sia stato indicato almeno un mezoz da sostituire
                if (carReplacement.OriginalPEriodCarIds == null
                    || carReplacement.OriginalPEriodCarIds.Count == 0)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_CarReplacement,
                            Title = "Nessun mezzo da sostituire",
                            Description = "Deve essere indicato almeno u mezzo da sostituire",
                            Id = carReplacement.CarReplacementId
                        });
                }
                else
                {
                    var periodCars = period.Cars.Where(c => carReplacement.OriginalPEriodCarIds.Contains(c.RunCarId)).Select(c => c.CarId);

                    // controlal che non sia stato indicato più volte lo stesso mezzo tra quelli da sosituire
                    if (periodCars.Distinct().Count() != periodCars.Count())
                    {
                        errors.Add(
                            new CheckResultItem()
                            {
                                Category = RunCheckResult.Category_CarReplacement,
                                Title = "Mezzi da sostituire doppi",
                                Description = "Sono presenti dei mezzi da sostituire indicati più di una volta.",
                                Id = carReplacement.CarReplacementId
                            });
                    }
                }

                // contorlla che sia stato indicato alemno un mezzo sosttituito
                if (carReplacement.ReplacedPEriodCarIds == null
                    || carReplacement.ReplacedPEriodCarIds.Count == 0)
                {
                    errors.Add(
                        new CheckResultItem()
                        {
                            Category = RunCheckResult.Category_CarReplacement,
                            Title = "Nessun mezzo sotitutivo",
                            Description = "Deve essere indicato almeno u mezzo sostitutivo",
                            Id = carReplacement.CarReplacementId
                        });
                }
                else
                {
                    var periodCars = period.Cars.Where(c => carReplacement.ReplacedPEriodCarIds.Contains(c.RunCarId)).Select(c => c.CarId);

                    // controlla che non sia stato indicato più volte lo stesso mezzo tra quelli sostituiti
                    if (periodCars.Distinct().Count() != periodCars.Count())
                    {
                        errors.Add(
                            new CheckResultItem()
                            {
                                Category = RunCheckResult.Category_CarReplacement,
                                Title = "Mezzi sostitutivi doppi",
                                Description = "Sono presenti dei mezzi sostitutivi indicati più di una volta.",
                                Id = carReplacement.CarReplacementId
                            });
                    }
                }
            }

            await Task.CompletedTask;
        }
        #endregion
        #endregion

        #region suspensions
        private async Task CheckSuspensionAsync(
            RunItem run,
            RunSuspension suspension,
            IList<CheckResultItem> errors,
            IList<CheckResultItem> warnings,
            IList<CheckResultItem> informations
            )
        {
            if (suspension.StartDate > suspension.EndDate)
            {
                errors.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Suspension,
                        Title = "Date sospensione incongruenti",
                        Description = "La data di inizio della sospensione è successiva  a quella di fine.",
                        Id = suspension.RunSuspensionId
                    });
            }
            if (string.IsNullOrWhiteSpace (suspension.SuspensionNote) == true)
            {
                informations.Add(
                    new CheckResultItem()
                    {
                        Category = RunCheckResult.Category_Suspension,
                        Title = "Sospensione senza descrizione",
                        Description = "Non è stata indicata una descrizione epr la sospensione",
                        Id = suspension.RunSuspensionId
                    });
            }

            await Task.CompletedTask;
        }
        #endregion
    }
}
