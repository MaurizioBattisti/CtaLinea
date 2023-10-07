using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Auth.Model;
using CtaLineaWebApi.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;

namespace CtaLineaWebApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/auth")]
    public class AuthController
        : ZControllerBase
    {
        private readonly IUserService _userService;
        // private readonly IUserInfo _userInfo;
        private readonly ISchedulerService _schedulerService;

        public AuthController(
            IUserService userService,
            ISchedulerService scheduleService
            // IUserInfo userInfo
            )
        {
            _userService = userService;
            _schedulerService = scheduleService;
            // _userInfo = userInfo;
        }

        [Route("login")]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            summary: "autentica un utente",
            description: "restituisce un JWT se l'utente è censito",
            Tags = new string[] { "Authorizaiton" }
            ),]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(
            [FromBody] AuthenticateRequest model
            )
        {
            var response = await _userService.AuthenticateAsync(model);

            if (response == null)
            {
                return BadRequest(
                    new ValidationProblemDetails (new Dictionary<string, string[]>())
                    {
                        Title = "Login Fallito",
                        Detail = "Utente o password errati"
                    });
            }

            return Ok(response);
        }

        [Route("password")]
        [HttpPost]
        [SwaggerOperation(
            summary: "Cambio password",
            description: "cambia la password dell'utente connesso",
            Tags = new string[] { "Authorizaiton" }
            )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Authorize(Policy = Constants.Policy_ChangePAssword)]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ChangePasswordModel model)
        {
            // controlla che la nuova e la vecchia password non siano uguali
            if (model?.OldPAssword.ToUpper () == model?.NewPassword.ToUpper())
            {
                return BadRequest(
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Cambio password falito",
                        Detail = "vecchia password e nuova password identiche o diverse solo per le maiuscole"
                    });
            }
            // chiama la funzione  di modifica della password
            var result = await this._userService.ChangePasswordASync(
                this.User.Identity.Name,
                model.OldPAssword, model.NewPassword
                );
            if (result.Changed == false)
            {
                return this.BadRequest (
                    new ValidationProblemDetails(new Dictionary<string, string[]>())
                    {
                        Title = "Cambio password falito",
                        Detail = result.Message
                    });
            }
            return this.NoContent();
        }
    }
}
