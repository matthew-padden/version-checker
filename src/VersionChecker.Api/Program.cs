using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace VersionChecker.Api;

public static class Program
{
    public static readonly string Namespace = typeof(Program).Namespace ?? string.Empty;
    public static readonly string AppName = Namespace[(Namespace.LastIndexOf('.', Namespace.LastIndexOf('.') - 1) + 1)..];
    public static readonly string Env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? Environments.Production;

    public static int Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.WithProperty("ApplicationContext", AppName)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        Log.Information($"Starting {AppName} in {Env} environment...");

        try
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((_, builder) => builder.AddConfiguration(LoadConfiguration(args)))
                .UseSerilog((context, services, configuration) =>
                    configuration
                        .ReadFrom.Configuration(context.Configuration)
                        .ReadFrom.Services(services)
                        .Enrich.FromLogContext()
                        .Enrich.WithProperty("ApplicationContext", AppName)
                        .Enrich.WithProperty("Environment", context.HostingEnvironment)
                        .WriteTo.Console(theme: AnsiConsoleTheme.Code, outputTemplate: "{Timestamp:HH:mm:ss} [{Level:u3}] {app}: {Message:lj}{NewLine}{Exception}"))
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
                .Build()
                .Run();

            Log.Information("Stopped cleanly");
            return 0;
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Program terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
        

    private static IConfiguration LoadConfiguration(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Env}.json", optional: true, reloadOnChange: true);

        if (args != null)
            builder.AddCommandLine(args);

        return builder.Build();
    }
}