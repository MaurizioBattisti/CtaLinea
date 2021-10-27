using CtaLinea.Application.Utility;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CtaLinea
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            builder.Services.AddOidcAuthentication(options =>
            {
                builder.Configuration.Bind(Constants.Config_Oidc, options.ProviderOptions);
            });


            // registra tutti i servizi aggiuntivi
            RegisterService(builder);

            builder.RootComponents.Add<App>(Constants.Scope_App);

            await builder.Build().RunAsync();
        }

        private static  WebAssemblyHostBuilder RegisterService (
            WebAssemblyHostBuilder builder)
        {
            // legge la configurazione
            var config = new CtaLineaApiConfiguration();
            builder.Configuration.GetSection(Constants.Config_CtaLineaApi).Bind(config);

            // We register a named HttpClient here for the API
            builder.Services.AddHttpClient(Constants.Scope_Http_Api)
                .AddHttpMessageHandler(sp =>
                {
                    var handler = sp.GetService<AuthorizationMessageHandler>()
                        .ConfigureHandler(
                            authorizedUrls: new[] { config.BaseAddress },
                            scopes:  config.Scopes ); return handler;
                });
            // we use the api client as default HttpClient
            builder.Services.AddScoped(
              sp => sp.GetService<IHttpClientFactory>().CreateClient(Constants.Scope_Http_Api));





            // servizi custom
            builder.Services.AddSingleton<IQueryUtilityService>
                (s => 
                {
                    return new QueryUtilityService(
                        s.GetService<IHttpClientFactory>().CreateClient("api"),
                        config
                        );
                });

            builder.Services.AddScoped<Radzen.DialogService>();

            return builder;
        }
    }
}
