using VersionChecker.Api.Model;

namespace VersionChecker.Api.Areas.DotNet.Models
{
    public class DotNetVersionDetail : VersionDetail
    {
        public string TargetFramework =>
            this.AdditionalProperties.FirstOrDefault(prop => prop.Key == "targetFramework").Value;

        public string TargetFrameworkMoniker =>
            this.AdditionalProperties.FirstOrDefault(prop => prop.Key == "tfm").Value;
    }
}
