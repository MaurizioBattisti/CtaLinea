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
using CtaLineaWebApi.Auth;
using CtaLinea.Model.ModelServices;
using CtaLineaWebApi.Application.Services;
using ZzSoft.CtaLinea.Dal.Services;
using CtaLineaWebApi.Application.Scheduler;
using NPOI.OpenXml4Net.OPC;
using Microsoft.AspNetCore.Identity;

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

            var emailConfig = Configuration
                .GetSection(Constants.Configuration_MailSender)
                .Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);



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

            // aggiunge il servizio epr il jwt token
            services.AddJWTTokenServices(this.Configuration);
            // aggiunge le policy di autorizzaizone
            services.AddAuthorizationPolicies();

            // servizio di interazione con lo scheduler di attivi5tà
            services.AddScoped<ISchedulerService, SchedulerService>()
                .AddScoped<ISchedulerTaskLogger, SchedulerTaskLogger>()
                ;

            // servizio di invio mail
            services.AddScoped<IMailSender, MailSender>()
                ;

			// per ultimo aggiunge lo scheduler
			services.AddSingleton<TaskScheduler, TaskScheduler>();
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

            // recupera il servizio dello scheduler per avviarlo
            var scheduler = app.ApplicationServices.GetService<TaskScheduler>();
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
            services.AddScoped<IAssociatesQueries,AssociatesQueries>()
                .AddScoped<IRunQueries,RunQueries>()
                .AddScoped<ICollectionPointsQueries,CollectionPointsQueries>()
                .AddScoped<ICalendaQueries,CalendaQueries>()
                .AddScoped<IContractsQueries, ContractsQueries>()
                .AddScoped<ITtServicesQueries,TtServicesQueries>()
                .AddScoped<ICostQueries, CostQueries> ()
                .AddScoped<IUtilityQueries, UtilityQueries> ()
                .AddScoped<ICalendarRepository, CalendarRepository>()
                .AddScoped<IForfaitQueries, ForfaitQueries> ()
                .AddScoped<IUsersQueries, UsersQueries> ()
                ;

            // repository 
            services.AddScoped<IUserRepository, UserRepository>()
                .AddScoped<IRunRepository, RunRepository>()
                .AddScoped<ITtServiceRepository, TtServiceRepository>()
                .AddScoped<ITagRepository, TagRepository>()
                .AddScoped<IContractRepository, ContractRepository>()
                .AddScoped<IUtilityREpository, UtilityREpository>()
                .AddScoped<IForfaitRepository, ForfaitRepository>()
                .AddScoped<ISchedulerTaskRepository, SchedulerTaskRepository> ()

                // altri servizi del DAL
                .AddScoped<INodeMatchService, NodeMatchService>()
                ;

            // servizi per il contorllo dei dati
            services.AddScoped<IRunChecker, RunChecker>()
                .AddScoped<ICompleteRunCheckerService, CompleteRunCheckerService>()
                .AddScoped<IContractChecker, ContractChecker> ()
                .AddScoped<ISchedulerTaskChecker, SchedulerTaskChecker>()
                ;

            return services;
        }
    }
}

