using System.Reflection;
using Serilog;
using SimpleSplit.Application;
using SimpleSplit.Infrastructure;
using SimpleSplit.WebApi.Swagger;
using Swashbuckle.AspNetCore.Filters;

// Prepare logging path and configuration
var binPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? String.Empty;
var logPath = Path.Combine(binPath, "Logs");
Environment.SetEnvironmentVariable("LOG_PATH", logPath);

var configuration = GetConfiguration(binPath);
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

try
{
    Log.Information("Starting up");
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    builder.Services
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices();

    // Add services to the container.
    builder.Services
        .AddControllers()
        .AddControllersAsServices();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options => options.SetupSwagger());
    builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SetupSwaggerUI());
    }

    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();
    app.UseAuthorization();

    app.MapControllers();

    // Please note that there is a bug in minimal APIs!!
    // https://github.com/dotnet/aspnetcore/issues/38185
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application start-up failed");
#if DEBUG
    Console.ReadKey(false);
#endif
}
finally
{
    Log.Information("Shutdown complete. Bye-bye!");
    Log.CloseAndFlush();
}

static IConfiguration GetConfiguration(string path)
    => new ConfigurationBuilder()
        .SetBasePath(path)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile(
            $"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json",
            optional: true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();

// To support tests!
public partial class Program { }