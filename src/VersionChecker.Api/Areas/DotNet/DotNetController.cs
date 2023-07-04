using Microsoft.AspNetCore.Mvc;
using VersionChecker.Api.Controller;
using VersionChecker.Api.Model;
using VersionChecker.Infrastructure;

namespace VersionChecker.Api.Areas.DotNet
{
    [Route("[controller]")]
    [ApiController]
    public class DotNetController : VersionCheckerController<DotNetVersionDetail>
    {

        public DotNetController(IRepository<DotNetVersionDetail> repository)
            : base(repository) { }

        /// <summary>
        /// Get the version details for a specific version.
        /// </summary>
        /// <param name="version">The version number to retrieve.</param>
        /// <returns>Details of the specific version.</returns>
        [HttpGet("{version}")]
        [ProducesResponseType(typeof(VersionDetail), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string version)
            => await GetVersion(version);


        /// <summary>
        /// Get the version details.
        /// </summary>
        /// <param name="tfm">Target framework moniker</param>
        /// <returns>Version details</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VersionDetail>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(VersionDetail), StatusCodes.Status200OK)]        
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Find([FromQuery] string tfm)
            => await GetByAdditionalProperty("tfm", tfm);
    }
}
