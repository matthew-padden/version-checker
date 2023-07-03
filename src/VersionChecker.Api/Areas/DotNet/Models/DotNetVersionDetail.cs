using VersionChecker.Api.Model;

namespace VersionChecker.Extensions.DotNet.Model
{
    public class DotNetVersionDetail : VersionDetail
    {
        public string TargetFrameworkMoniker =>
            this.AdditionalProperties.FirstOrDefault(prop => prop.Key == "tfm").Value;
    }
}
