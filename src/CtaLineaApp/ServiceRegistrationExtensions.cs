using CtaLineaApp.Application.Services.Helper;
using CtaLineaApp.Application.Services.Account;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using System.Net.Http;
using CtaLineaApp.Application.Services.Utility;
using CtaLineaApp.Application.Services.Run;
using CtaLineaApp.Application.Services.Base;

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
                .AddScoped<IAccountService, AccountService>()
                // .AddScoped<IAlertService, AlertService>()
                .AddScoped<IHttpService, HttpService>()
                .AddScoped<ILocalStorageService, LocalStorageService>()
                .AddScoped<IQueryUtilityService, QueryUtilityService>()
                
                // servizio per la gestiond ei contratti
                .AddScoped<IContractService, ContractService> ()
                // Serivizo per la gestiondei calendari
                .AddScoped<ICalendarService, CalendarService>()
                // serivi per gestire le corse
                .AddScoped<IRunService, RunService> ()
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
