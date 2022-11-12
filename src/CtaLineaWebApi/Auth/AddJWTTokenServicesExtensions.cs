using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using CtaLineaWebApi.Auth.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;

namespace CtaLineaWebApi.Auth
{
    public static class AddJWTTokenServicesExtensions
    {
        public static void AddJWTTokenServices(
            this IServiceCollection services,
            IConfiguration configuration,
            string sectionName = Costants.JwtOptions_SectionName)
        {
            // Add Jwt Setings
            var jwtOpt = configuration.GetSection(sectionName);
            services.Configure<JwtOptions>(
                jwtOpt
                );
            var jwtOptions = jwtOpt.Get<JwtOptions>();

            // prima erano singleton ora sono scoped
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserService, UserService>();

            // services.AddSingleton(bindJwtSettings);
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = jwtOptions.ValidateIssuerSigningKey,
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtOptions.IssuerSigningKey)),
                    ValidateIssuer = jwtOptions.ValidateIssuer,
                    ValidIssuer = jwtOptions.ValidIssuer,
                    ValidateAudience = jwtOptions.ValidateAudience,
                    ValidAudience = jwtOptions.ValidAudience,
                    RequireExpirationTime = jwtOptions.RequireExpirationTime,
                    ValidateLifetime = jwtOptions.RequireExpirationTime,
                    ClockSkew = TimeSpan.FromDays(1),
                };
            });
            services.AddAuthorization();
        }
    }
}