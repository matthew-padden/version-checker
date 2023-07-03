using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using VersionChecker.Api.Application;
using VersionChecker.Api.Model;
using VersionChecker.Api.Queries;

namespace VersionChecker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DotNetController : ControllerBase
    {
        private readonly IRepository repository;

        public DotNetController(IRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get() => 
            Ok(await repository.GetAsync());

        [HttpGet("{version}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetVersion(string version)
        {
            var dotNetVersion = await repository.GetAsync(version);
            if (dotNetVersion == null)
                return BadRequest();

            return Ok(new VersionResponse(dotNetVersion.IsInSupport));
        }
    }
}
