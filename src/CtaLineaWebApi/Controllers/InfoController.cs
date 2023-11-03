using CtaLineaWebApi.Application.Scheduler;
using CtaLineaWebApi.Auth.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ZzSoft.Api.Utility.Base;

namespace CtaLineaWebApi.Controllers
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/info")]
    public class InfoController
        : ZControllerBase
    {
        private readonly ISchedulerService _schedulerService;
        private readonly Application.Scheduler.TaskScheduler _scheduler;

		public InfoController (
            Application.Scheduler.TaskScheduler scheduler,
			ISchedulerService schedulerService)
        {
            _scheduler = scheduler;
            _schedulerService = schedulerService;   
        }

        [Route("versions")]
        [HttpGet]
        [AllowAnonymous]
        [SwaggerOperation(
            summary: "Versions",
            description: "Restiusce le versioni del server",
            Tags = new string[] { "About" }
            ),]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<IEnumerable<KeyValuePair<string, string>>>> GetVersionsAsync(
            )
        {
            var list = new List<KeyValuePair<string, string>>();
            list.AddRange(
                this.GetInfo()
                );

            return Ok(list);
        }

        [Route("tick")]
        [HttpPost]
        [AllowAnonymous]
        [SwaggerOperation(
            summary: "Tick",
            description: "Esegue un tick dello scheduelr",
            Tags = new string[] { "Scheduler" }
            ),]
        [ProducesResponseType(StatusCodes.Status202Accepted)]
        public async Task<IActionResult> SchedulerTichAsync(
            )
        {
            await Task.CompletedTask;

            _scheduler.DoTick();
            /*
            var data = _scheduler.GetActualScheduledTasks();
			return this.Ok(data);
            */
            return this.NoContent();
        }

		private IEnumerable<KeyValuePair<string, string?>> GetInfo()
        {
            // versione assembly
            var assembly = Assembly.GetExecutingAssembly();
            yield return new KeyValuePair<string, string>("Server - Web API",
                null
                );
            yield return new KeyValuePair<string, string>("Versione",
                assembly.GetName()?.Version?.ToString()
                );
        }
    }
}
