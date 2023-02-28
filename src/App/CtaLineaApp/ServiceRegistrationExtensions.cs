using CtaLineaApp.Application.Helpers;
using CtaLineaApp.Application.Services.Account;
using CtaLineaApp.Application.Services.Query;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

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
                .AddScoped<ILocalStorageService, LocalStorageService>()

                // My 
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<IQueryUtilityService, QueryUtilityService>()
                .AddScoped<IAccountService, AccountService>()

                /*
                // Cfc Routing
                // .AddCECRouting()

                .AddScoped(typeof(IAutoLoadDataList<>), typeof(AutoLoadDataList<>))

                .AddScoped<IApplicationSettings, ApplicationSettings>()
                // .AddScoped<IAlertService, AlertService>()
                .AddScoped<ICostService, CostServic>()
                .AddScoped<IUtilityService, UtilityService>()
                .AddScoped<ITagsService, TagsService>()

                // servizio per la gestion dei contratti
                .AddScoped<IContractService, ContractService>()
                // Serivizo per la gestiondei calendari
                .AddScoped<ICalendarService, CalendarService>()
                // serivi per gestire le corse
                .AddScoped<IRunModelService, RunModelService>()

                // repository dei dati
                .AddScoped<IRunRepository, RunRepository>()

                // servizio di controllo delle corse
                .AddScoped<IRunChecker, RunChecker>()
                */
                ;

            return builder;
        }
    }
}
