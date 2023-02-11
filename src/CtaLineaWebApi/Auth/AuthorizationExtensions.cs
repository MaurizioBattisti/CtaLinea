using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace CtaLineaWebApi.Auth
{
    internal static class AuthorizationExtensions
    {
        public static IServiceCollection AddAuthorizationPolicies(
            this IServiceCollection services)
        {
            services.AddAuthorization(options =>
           {
               options.InvokeHandlersAfterFailure = false;

             /*
            // aggiunge la policy per il cambio password
            options.AddPolicy(Constants.AuthPolicy_ChangePAssword, builder =>
                  {
                      builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                      builder.RequireClaim(ClaimTypes.Name);
                  });
             */

            // verifica la necessità di modificare la password
            Func<AuthorizationHandlerContext, bool> handleMustChangePwd = (context) =>
                  {
                      bool valid = false;
                   // controlla che l'utente non deva modificare la password
                   var identity = context.User.Identity as ClaimsIdentity;
                      if (identity != null)
                      {
                          bool mustChange = identity.Claims
                              .Where(c => c.Type == ClaimTypes.UserData)
                              .Select(c => bool.Parse(c.Value))
                              .SingleOrDefault();
                          valid = mustChange == false;
                      }

                      return valid;
                  };

               options.AddPolicy("PAT", builder =>
                   {
                       builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

                       builder.RequireClaim(ClaimTypes.Name);
                       builder.RequireAssertion(
                           handleMustChangePwd);
                       builder.RequireRole("PAT_Users");
                   });


           });

            return services;
        }
    }
}
