using CtaLineaWebApi.Auth.Model;
using CtaLineaWebApi.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
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

        public AuthController(
            IUserService userService
            // IUserInfo userInfo
            )
        {
            _userService = userService;
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
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(response);
        }

        /*
        [Route("password")]
        [HttpPost]
        [SwaggerOperation(
            summary: "autentica un utente",
            description: "restituisce un JWT se l'utente è censito",
            Tags = new string[] { "Authorizaiton" }
            )]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        // [Authorize(Policy = CtaLineaConstants.AuthPolicy_ChangePAssword)]
        public async Task<IActionResult> ChangePasswordAsync(
            [FromBody] ChangePasswordModel model)
        {
            // controlla che la nuova e la vecchia password non siano uguali
            if (model?.OldPAssword == model?.NewPassword)
            {
                return this.BadRequest("vecchia password e  nuova password identiche");
            }

            // chiama la funzione  di modifica della password
            


            await Task.CompletedTask;
            return this.NoContent();
        }
        */

    }
}
