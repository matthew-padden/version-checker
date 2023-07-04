using System.Text.Json;
using VersionChecker.Api.Model;

namespace VersionChecker.Api.Infrastructure
{
    public static class FileReader
    {
        public async static Task<VersionInfo<T>> ReadJson<T>(string filePath) where T : IVersionDetail
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File {filePath} not found");

            using FileStream openStream = File.OpenRead(filePath);
            return await JsonSerializer.DeserializeAsync<VersionInfo<T>>(openStream, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }
}
