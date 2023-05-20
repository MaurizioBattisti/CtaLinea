using CtaLinea.Model.Base;
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

			   // aggiunge la policy per il cambio password
			   options.AddPolicy(Constants.Policy_ChangePAssword, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
				   builder.RequireClaim(ClaimTypes.Name);
			   });

			   // Tasks
			   options.AddPolicy(Constants.Policy_Tasks, builder =>
                {
                    builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

                    builder.RequireClaim(ClaimTypes.Name);
                    builder.RequireAssertion(
                        handleMustChangePwd);
                    builder.RequireRole(UserRoles.Role_Takss);
                });
			   // utenti
			   options.AddPolicy(Constants.Policy_Users, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(UserRoles.Role_Users);
			   });

			   // Visaulizzazione dei dati
			   // bastga essere autenticati e avere uno dei ruoli indicati
			   options.AddPolicy(Constants.Policy_ViewData, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(UserRoles.All);
			   });
			   // gestione dei dati
			   options.AddPolicy(Constants.Policy_ManageData, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(UserRoles.Role_Manage);
			   });

			   // Planning
			   options.AddPolicy(Constants.Policy_Planning, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(UserRoles.Role_Planning);
			   });
			   // visualizzaizone corse
			   options.AddPolicy(Constants.Policy_RunView, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(
					   UserRoles.Role_View,
					   UserRoles.Role_Edit,
					   UserRoles.Role_Manage);
			   });
			   // visualizzaizone corse
			   options.AddPolicy(Constants.Policy_RunEdit, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(
					   UserRoles.Role_Edit,
					   UserRoles.Role_Manage);
			   });
			   // costi
			   options.AddPolicy(Constants.Policy_Costs, builder =>
			   {
				   builder.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);

				   builder.RequireClaim(ClaimTypes.Name);
				   builder.RequireAssertion(
					   handleMustChangePwd);
				   builder.RequireRole(UserRoles.Role_Costs);
			   });

			   


		   });

            return services;
        }
    }
}
