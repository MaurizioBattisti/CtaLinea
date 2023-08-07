using CtaLineaApp.Application.Services.Helper;
using CtaLineaApp.Application.Services.Account;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using CtaLineaApp.Application.Services.Utility;
using CtaLineaApp.Application.Services.Run;
using CtaLineaApp.Application.Services.Base;
using Radzen;
using CtaLineaApp.Pages.Services;
using CEC.Routing;
using CtaLinea.Model.ModelServices;
using CtaLineaApp.Application.Services.Costs;
using CtaLineaApp.Application.Services.Utilities;
using CtaLineaApp.Application.Services.Contab;
using CtaLinea.Model.Base;
using System.Security.Claims;

namespace CtaLineaApp
{
    internal static class ServiceRegistrationExtensions
    {
        internal static WebAssemblyHostBuilder RegisterService(
            this WebAssemblyHostBuilder builder)
        {
            // legge la configurazione
            var config = new CtaLineaApiConfiguration();
            builder.Configuration.GetSection(Constants.Config_CtaLineaApi).Bind(config);

			// configure http client
			builder.Services.AddScoped(x => {
                var apiUrl = new Uri(config.BaseAddress);
                return new HttpClient() { BaseAddress = apiUrl };
            });

            builder.Services
                // agginge i servizi globali
                .AddSingleton<IGlobalFiltersService, GlobalFiltersService> ()

                // radzen registration
                .AddScoped<NotificationService>()
                .AddScoped<DialogService>()
                .AddScoped<ContextMenuService>()

                // Cfc Routing
                .AddCECRouting()

                // My
                .AddScoped(typeof(IAutoLoadDataList<>), typeof(AutoLoadDataList<>))

                // servizio di esportaizone in excel
                .AddScoped<IExcelExporterService, ExcelExporterService>()

                .AddScoped<IApplicationSettings, ApplicationSettings>()
                .AddScoped<ICurrentUserService, CurrentUserService>()
                .AddScoped<IAccountService, AccountService>()
                // .AddScoped<IAlertService, AlertService>()
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IQueryUtilityService, QueryUtilityService>()
                .AddScoped<ICostService, CostServic>()
                .AddScoped<IUtilityService, UtilityService>()
                .AddScoped<ITagsService, TagsService>()
                .AddScoped<IUserService, UserService>()

                // servizio per la gestion dei contratti
                .AddScoped<IContractService, ContractService>()
                // Serivizo per la gestiondei calendari
                .AddScoped<ICalendarService, CalendarService>()
                // serivi per gestire le corse
                .AddScoped<IRunModelService, RunModelService>()
                // servizi per la gestione dei task schedulati
                .AddScoped<IScheduledTasksService, ScheduledTasksService>()

                // repository dei dati
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<IForfaitService, ForfaitService>()

                // servizio di controllo delle corse
                .AddScoped<IRunChecker, RunChecker> ()
                .AddScoped<IContractChecker, ContractChecker> ()
                .AddScoped<ISchedulerTaskChecker, SchedulerTaskChecker> ()
                .AddScoped<IUserChecker, UserChecker>()
                ;

            /*
            // servizi custom
            builder.Services.AddSingleton<IQueryUtilityService>
                (s =>
                {
                    return new QueryUtilityService(
                        s.GetService<IHttpClientFactory>().CreateClient("api"),
                        config
                        );
                });
            */
            builder.Services.AddScoped<Radzen.DialogService>();

            return builder;
        }
    }
}
