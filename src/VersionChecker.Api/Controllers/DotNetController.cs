using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using VersionChecker.Api.Model;

namespace VersionChecker.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DotNetController : ControllerBase
    {
        private readonly IConfiguration configuration;        

        public DotNetController(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            var filename = configuration["VersionFileNames:DotNet"];
            var dotNetVersions = await ReadJsonFileAsync(filename);
            return Ok(dotNetVersions);
        }

        private async Task<List<VersionInfo>> ReadJsonFileAsync(string filePath)
        {
            using FileStream openStream = System.IO.File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<List<VersionInfo>>(
                openStream,
                options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }
    }
}
