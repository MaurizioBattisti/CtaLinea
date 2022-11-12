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
    // [Authorize(Roles ="NOL PASSA")]
    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController
        : ZControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(
            IUserService userService
            )
        {
            _userService = userService;
        }
        
        [Route("list")]
        [HttpGet]
        [SwaggerOperation(
            summary: "autentica un utente",
            description: "restituisce un JWT se l'utente è censito",
            Tags = new string[] { "Authorizaiton" }
            ),]
        [ProducesResponseType(
            StatusCodes.Status200OK,
            Type = typeof(IEnumerable<string>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetUSersAsync(
            )
        {
            var id = this.User.Identity;

            var result = new List<string>();

            await Task.CompletedTask;

            return Ok(result);
        }
    }
}
