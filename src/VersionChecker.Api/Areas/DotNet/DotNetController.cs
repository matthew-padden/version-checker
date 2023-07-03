using Microsoft.AspNetCore.Mvc;
using VersionChecker.Api.Model;
using VersionChecker.Extensions.DotNet.Model;
using VersionChecker.Infrastructure;

namespace VersionChecker.Api.Areas.DotNet
{
    [Route("[controller]")]
    [ApiController]
    public class DotNetController : ControllerBase
    {
        protected readonly IRepository<DotNetVersionDetail> repository;

        public DotNetController(IRepository<DotNetVersionDetail> repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tfm">Target Framework Moniker</param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(VersionDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get([FromQuery] string tfm)
        {
            if (string.IsNullOrEmpty(tfm))
                return Ok(await repository.GetAsync());

            var dotNetVersion = await repository.GetByAdditionalPropertyAsync("tfm", tfm);
            if (dotNetVersion == null)
                return BadRequest();

            return Ok(dotNetVersion);
        }
    }
}
