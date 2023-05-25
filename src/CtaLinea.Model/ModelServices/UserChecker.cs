using CtaLinea.Model.Base;
using CtaLinea.Model.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea.Model.ModelServices
{
    public class UserChecker :
        IUserChecker
    {
        public async Task<CheckResult> CheckAsync(NewUserModel user)
        {
            return await this.CheckUserAsync(
                user,
                async (u, errors, warnings, informations) =>
                {
                    await Task.CompletedTask;

                    if (string.IsNullOrEmpty(u.UserName) == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicato il nome utente",
                            Title = string.Empty,
                        });
                    }
                    if (string.IsNullOrEmpty(u.Password) == true
                        && u.Interactive == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicata la password da assegnare all'utente",
                            Title = string.Empty,
                        });
                    }
                    if (string.IsNullOrEmpty(u.Description) == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "La descrizione dell'utente non può essere lasciata vuota",
                            Title = string.Empty,
                        });
                    }
                    if (string.IsNullOrEmpty(u.Email) == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicata una mail da associare all'utente",
                            Title = string.Empty,
                        });
                    }
                    if (u.Roles == null || u.Roles.Count() == 0)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicato almeno un ruolo da assegnare all'utente",
                            Title = string.Empty,
                        });
                    }
                });
        }

        public async Task<CheckResult> CheckAsync(EditUSerModel user)
        {
            return await this.CheckUserAsync(
                user,
                async (u, errors, warnings, informations) =>
                {
                    await Task.CompletedTask;

                    if (string.IsNullOrEmpty(u.Description) == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "La descrizione dell'utente non può essere lasciata vuota",
                            Title = string.Empty,
                        });
                    }
                    if (string.IsNullOrEmpty(u.Email) == true)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicata una mail da associare all'utente",
                            Title = string.Empty,
                        });
                    }
                    if (u.Roles == null || u.Roles.Count() == 0)
                    {
                        errors.Add(new CheckResultItem()
                        {
                            Category = string.Empty,
                            Description = "Deve essere indicato almeno un ruolo da assegnare all'utente",
                            Title = string.Empty,
                        });
                    }
                });
        }

        private async Task<CheckResult> CheckUserAsync<T>(
            T user,
            Func<T, List<CheckResultItem>, List<CheckResultItem>, List<CheckResultItem>, Task> checkASync
            )
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

            await checkASync(user, errors, warnings, informations);

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
