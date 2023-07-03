using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using VersionChecker.Api.Model;
using VersionChecker.Api.Queries;
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

        private readonly VersionInfo DotNetVersionInfo = new()
        {
            Details = new List<VersionDetail>
            {
                new VersionDetail {
                    Version = "7.0",
                    ReleaseDate = new DateTime(2020, 11, 10),
                    EndOfSupportDate = DateTime.UtcNow.AddMonths(12)
                },
                new VersionDetail
                {
                    Version = "3.1",
                    ReleaseDate = new DateTime(2019, 12, 3),
                    EndOfSupportDate = new DateTime(2022, 12, 3)
                },
                new VersionDetail
                {
                    Version = "2.1",
                    ReleaseDate = new DateTime(2018, 5, 30),
                    EndOfSupportDate = new DateTime(2021, 8, 21)
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

            var versionInfo = JsonSerializer.Deserialize<VersionInfo>(content, options: jsonSerializerOptions);

            Assert.NotNull(versionInfo);
            Assert.Equal(DotNetVersionInfo.Details.Count, versionInfo.Details.Count);

            for (var i = 0; i < DotNetVersionInfo.Details.Count; i++)
            {
                Assert.Equal(DotNetVersionInfo.Details[i].Version, versionInfo.Details[i].Version);
                Assert.Equal(DotNetVersionInfo.Details[i].ReleaseDate, versionInfo.Details[i].ReleaseDate);
                Assert.Equal(DotNetVersionInfo.Details[i].EndOfSupportDate, versionInfo.Details[i].EndOfSupportDate);
            }
        }

        [Fact]
        public async Task Get_DotNetVersion_Exists_ReturnSuccess()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet/7.0"));
            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task Get_DotNetVersion_NotExist_ReturnsBadRequest()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet/45.0"));
            Assert.True(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Get_DotNetVersion_InSupport_ReturnsTrue()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet/7.0"));
            var content = await response.Content.ReadAsStringAsync();
            var versionResponse = JsonSerializer.Deserialize<VersionResponse>(content, options: jsonSerializerOptions);

            Assert.True(versionResponse.IsInSupport);
        }

        [Fact]
        public async Task Get_DotNetVersion_NotInSupport_ReturnsFalse()
        {
            var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, "/dotnet/5.0"));
            var content = await response.Content.ReadAsStringAsync();
            var versionResponse = JsonSerializer.Deserialize<VersionResponse>(content, options: jsonSerializerOptions);

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
