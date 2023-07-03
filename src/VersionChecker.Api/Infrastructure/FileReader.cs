using System.Text.Json;
using VersionChecker.Api.Model;

namespace VersionChecker.Api.Infrastructure
{
    public static class FileReader
    {
        public async static Task<VersionInfo<T>> ReadJson<T>(string filePath) where T : IVersionDetail
        {
            using FileStream openStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<VersionInfo<T>>(openStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
