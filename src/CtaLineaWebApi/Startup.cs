using CtaLineaWebApi.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using ZzSoft.CtaLinea.Dal.Context;
using ZzSoft.CtaLinea.Dal.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using ZzSoft.CtaLinea.Dal.Repositories;
using Microsoft.IdentityModel.Tokens;
using CtaLineaWebApi.Utility.BackGround;

namespace CtaLineaWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(
            IServiceCollection services)
        {
            services.AddControllers();

            // NON SPOSTARE LA DEFINIZIONE DEL CORS DA QUI
            services.AddCors(policy =>
            {
                policy.AddPolicy(Constants.CorsPolicyName, opt => opt
                .AllowAnyOrigin()
                // .SetIsOriginAllowed((host) => true)
                .AllowAnyHeader()
                .AllowAnyMethod()
                // .AllowCredentials()
                .WithExposedHeaders("*")
                );
            });

            /*
            services.AddMvcCore()
                .AddApiExplorer();
             */
            var oidcConfig = new IdentityServerConfiguration();
            this.Configuration.GetSection(Constants.ConfigSection_IdentityServer)
                .Bind(oidcConfig);
            
            // Identity Service
            services.AddAuthentication(
                JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.Authority = oidcConfig.Authority;
                  options.Audience = oidcConfig.Audience;
              });
            /*
            "CtaLinea_WebAPi"
            "wi8wDyV0E95Xp99fQM7FQHkYx8MaY+1yrzoYv9aw8mk="
            */

            // configura  le dimensioni  per  il sistema di upload dei fiel excel
            services.Configure<FormOptions>(o => {
                o.ValueLengthLimit = int.MaxValue;
                o.MultipartBodyLengthLimit = int.MaxValue;
                o.MemoryBufferThreshold = int.MaxValue;
            });

            // swagger
            this.AddSwagger(services);

            // registra il mediator con tutti i tipi 
            services.AddMediatR(
                System.Reflection.Assembly.GetAssembly(typeof(ILogger)),
                System.Reflection.Assembly.GetAssembly(typeof(Startup)),
                System.Reflection.Assembly.GetAssembly(typeof(ICalendaQueries))
                );

            // Gestione dei servizi in backgrouond
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            // aggiunge utti i servizi custom
            services.AddCustomServices(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            // NON SPOSTARE LA DEFINIZIONE CORS DA QUI
            app.UseCors(Constants.CorsPolicyName);

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            // Swagger
            app.UseSwagger(c =>
            {
                c.RouteTemplate = "api-docs/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/api-docs/v1/swagger.json", "Cta Linea API V1");
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                var groupName = "v1";

                options.SwaggerDoc(groupName, new OpenApiInfo
                {
                    Title = $"CTA Linea API {groupName}",
                    Version = groupName,
                    Description = "CTA Linea API",
                    Contact = new OpenApiContact
                    {
                        Name = "CTA Consorzio Trentino Autonoleggiatori",
                        Email = string.Empty,
                        Url = new Uri("https://ctatn.it/"),
                    }
                });
            });
        }
    }

    internal static class ServiceRegistrationExtensions
    {
        internal static IServiceCollection AddCustomServices (
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            var ctaLinaDbConf = new CtaLineaDbContextConfiguration();
            configuration.GetSection(Constants.ConfigSection_CtaLineaDb)
                .Bind(ctaLinaDbConf);
            services.AddSingleton<CtaLineaDbContextConfiguration>(ctaLinaDbConf);

            // registra il contesto di database
            services.AddScoped<CtaDbContext, CtaDbContext>();



            

            // servizi di qeury al DB
            services.AddScoped<
                IAssociatesQueries,
                AssociatesQueries>();
            services.AddScoped<
                ICollectionPointsQueries,
                CollectionPointsQueries>();
            services.AddScoped<
                ICategoriesQueries,
                CategoriesQueries>();
            services.AddScoped<
                ICalendaQueries,
                CalendaQueries>();
            services.AddScoped<
                IRunTypesQueries,
                RunTypesQueries>();
            services.AddScoped<
                IImportQueries,
                ImportQueries>();
            services.AddScoped<
                ITtServicesQueries,
                TtServicesQueries>();

            // repository per le importazioni da TT
            services.AddScoped<
                ITtServiceRepository,
                TtServiceRepository>();


            return services;
        }
    }

}

