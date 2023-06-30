using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;

namespace VersionChecker.Api.Tests;

public class TestClientProvider : IDisposable
{
    private readonly TestServer server;

    public IConfiguration Configuration;

    public HttpClient Client { get; }

    public TestClientProvider()
    {
        server = new TestServer(new WebHostBuilder()
            .UseEnvironment("Test")
            .UseConfiguration(new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("appsettings.Test.json")
                .Build())
            .UseStartup<Startup>());

        this.Client = server.CreateClient();
        this.Configuration = server.Host.Services.GetService(typeof(IConfiguration)) as IConfiguration;
    }

    public void Dispose()
    {
        this.Client.Dispose();
        this.server.Dispose();
    }
}