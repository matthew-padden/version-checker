using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VersionChecker.Extensions.DotNet.Model;
using VersionChecker.Api.Model;
using Xunit;

namespace VersionChecker.Api.Tests.EndToEnd
{
    public class DotNetTests : IClassFixture<TestClientProvider>, IDisposable
    {
        private readonly HttpClient client;
        private readonly IConfiguration configuration;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly VersionInfo<DotNetVersionDetail> DotNetVersionInfo = new()
        {
            RefreshDateTime = DateTime.UtcNow,
            Details = new List<DotNetVersionDetail>
            {
                new DotNetVersionDetail {
                    Version = ".NET 7",
                    ReleaseDate = new DateTime(2022, 11, 08),                    
                    EndOfSupportDate = null,
                    AdditionalProperties = new Dictionary<string, string>
                    {
                        { "tfm", "net7.0" }
                    }
                },
                new DotNetVersionDetail
                {
                    Version = ".NET 6 (LTS)",
                    ReleaseDate = new DateTime(2021, 11, 09),
                    EndOfSupportDate = new DateTime(2024, 11, 12),
                    AdditionalProperties = new Dictionary<string, string>
                    {
                        { "tfm", "net6.0" }
                    }
                },
                new DotNetVersionDetail
                {
                    Version = ".NET 5",
                    ReleaseDate = new DateTime(2020, 10, 08),
                    EndOfSupportDate = new DateTime(2022, 05, 10),
                    AdditionalProperties = new Dictionary<string, string>
                    {
                        { "tfm", "net5.0" }
                    }
                }
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
        public async Task Get_DotNet_ReturnVersionInfo()
        {
            var path = configuration["VersionFileNames:DotNet"];

            var request = new HttpRequestMessage(HttpMethod.Get, "/dotnet");
            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var versionDetails = JsonSerializer.Deserialize<List<DotNetVersionDetail>>(content, options: jsonSerializerOptions);

            Assert.NotNull(versionDetails);
            Assert.Equal(DotNetVersionInfo.Details.Count, versionDetails.Count);

            for (var i = 0; i < DotNetVersionInfo.Details.Count; i++)
            {
                Assert.Equal(DotNetVersionInfo.Details[i].Version, versionDetails[i].Version);
                Assert.Equal(DotNetVersionInfo.Details[i].ReleaseDate, versionDetails[i].ReleaseDate);
                Assert.Equal(DotNetVersionInfo.Details[i].EndOfSupportDate, versionDetails[i].EndOfSupportDate);
            }
        }

        [Fact]
        public async Task Get_DotNetVersion_Exists_ReturnSuccess()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet?tfm=net7.0"));
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Get_DotNetVersion_NotExist_ReturnsBadRequest()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet?tfm=net45.0"));
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_DotNetVersion_InSupport_ReturnsTrue()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet?tfm=net7.0"));
            var content = await response.Content.ReadAsStringAsync();
            var versionResponse = JsonSerializer.Deserialize<DotNetVersionDetail>(content, options: jsonSerializerOptions);

            Assert.True(versionResponse.IsInSupport);
        }

        [Fact]
        public async Task Get_DotNetVersion_NotInSupport_ReturnsFalse()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet?tfm=net5.0"));
            var content = await response.Content.ReadAsStringAsync();
            var versionResponse = JsonSerializer.Deserialize<DotNetVersionDetail>(content, options: jsonSerializerOptions);

            Assert.False(versionResponse.IsInSupport);
        }

        private void WriteVersionsFile(string path)
        {
            var json = JsonSerializer.Serialize(
                DotNetVersionInfo,
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
