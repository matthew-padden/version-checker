using VersionChecker.Api.Infrastructure;
using VersionChecker.Infrastructure;

namespace VersionChecker.Api.Areas.DotNet
{
    public class DotNetRepository : IRepository<DotNetVersionDetail>
    {
        private readonly IConfiguration configuration;

        public DotNetRepository(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<DotNetVersionDetail>> GetAsync()
        {
            var versionInfo = await FileReader.ReadJson<DotNetVersionDetail>(configuration["VersionFileNames:DotNet"]);
            return versionInfo.Details;

            // Possibility of scraping the data we need from https://learn.microsoft.com/en-us/dotnet/standard/frameworks but microsoft doesn
            // not allow scraping from its websites without written consent.
        }


        public async Task<DotNetVersionDetail> GetByAdditionalPropertyAsync(KeyValuePair<string, string> property)
        {
            var versionDetails = await GetAsync();
            return versionDetails.FirstOrDefault(det => det.AdditionalProperties.ContainsKey(property.Key) && det.AdditionalProperties[property.Key] == property.Value);
        }
    }
}
