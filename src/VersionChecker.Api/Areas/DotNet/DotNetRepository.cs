using VersionChecker.Api.Infrastructure;
using VersionChecker.Extensions.DotNet.Model;
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
        }


        public async Task<DotNetVersionDetail> GetByAdditionalPropertyAsync(string property, string value)
        {
            var versionDetails = await GetAsync();
            return versionDetails.FirstOrDefault(det => det.AdditionalProperties.ContainsKey(property) && det.AdditionalProperties[property] == value);
        }
    }
}
