using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VersionChecker.Api.Model;
using Xunit;

namespace VersionChecker.Api.Tests.EndToEnd
{
    public class DotNetTests : IClassFixture<TestClientProvider>, IDisposable
    {
        private readonly HttpClient client;
        private readonly IConfiguration configuration;

        private readonly List<VersionInfo> DotNetVersions = new List<VersionInfo>
        {
            new VersionInfo
            {
                Version = "5.0.0",
                ReleaseDate = new DateTime(2020, 11, 10),
                EndOfSupportDate = new DateTime(2022, 11, 10)
            },
            new VersionInfo
            {
                Version = "3.1.0",
                ReleaseDate = new DateTime(2019, 12, 3),
                EndOfSupportDate = new DateTime(2022, 12, 3)
            },
            new VersionInfo
            {
                Version = "2.1.0",
                ReleaseDate = new DateTime(2018, 5, 30),
                EndOfSupportDate = new DateTime(2021, 8, 21)
            }
        };

        public DotNetTests(TestClientProvider clientProvider)
        {
            this.client = clientProvider.Client;
            this.configuration = clientProvider.Configuration;

            WriteVersionsFile(configuration["VersionFileNames:DotNet"]);
        }

        [Fact]
        public async Task Get_DotNet_ReturnsSuccess()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "/dotnet");
            var response = await client.SendAsync(request);
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Get_DotNet_ReturnListOfDotNetVersions()
        {
            var path = configuration["VersionFileNames:DotNet"];

            var request = new HttpRequestMessage(HttpMethod.Get, "/dotnet");
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var versionInfos = JsonSerializer.Deserialize<List<VersionInfo>>(content, options: new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.NotNull(versionInfos);
            Assert.Equal(DotNetVersions.Count, versionInfos.Count);

            for (var i = 0; i < DotNetVersions.Count; i++)
            {
                Assert.Equal(DotNetVersions[i].Version, versionInfos[i].Version);
                Assert.Equal(DotNetVersions[i].ReleaseDate, versionInfos[i].ReleaseDate);
                Assert.Equal(DotNetVersions[i].EndOfSupportDate, versionInfos[i].EndOfSupportDate);
            }
        }

        private void WriteVersionsFile(string path)
        {
            var json = JsonSerializer.Serialize(
                DotNetVersions,
                options: new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            File.WriteAllText(path, json);
        }

        private void DeleteVersionsFile(string path)
            => File.Delete(path);

        public void Dispose()
        {
            DeleteVersionsFile(configuration["VersionFileNames:DotNet"]);
        }
    }
}
