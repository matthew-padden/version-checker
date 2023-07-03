using System.Text.Json;
using VersionChecker.Api.Model;

namespace VersionChecker.Api.Application
{
    public interface IRepository
    {
        Task<VersionInfo> GetAsync();
        Task<VersionDetail> GetAsync(string version);
    }

    public class Repository : IRepository
    {
        private readonly IConfiguration configuration;

        public Repository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<VersionInfo> GetAsync() =>
            await this.ReadJsonFileAsync();

        public async Task<VersionDetail> GetAsync(string version)
        {
            var info = await this.ReadJsonFileAsync();
            return info.Details.FirstOrDefault(d => d.Version == version);
        }

        private async Task<VersionInfo> ReadJsonFileAsync()
        {
            using FileStream openStream = File.OpenRead(configuration["VersionFileNames:DotNet"]);
            return await JsonSerializer.DeserializeAsync<VersionInfo>(
                openStream,
                options: new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
        }        
    }
}
