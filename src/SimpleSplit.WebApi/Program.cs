using System.Reflection;
using Autofac.Extensions.DependencyInjection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using SimpleSplit.Application;
using SimpleSplit.Application.Features.Security;
using SimpleSplit.Infrastructure;
using SimpleSplit.Infrastructure.Persistence;
using SimpleSplit.WebApi;
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
    builder.Host
        .UseServiceProviderFactory(new AutofacServiceProviderFactory())
        .UseSerilog();

    builder.Services
        .AddInfrastructureServices(builder.Configuration)
        .AddApplicationServices(builder.Configuration);

    // Add CORS settings
    var corsSettings = builder.Configuration.GetSection("CorsSettings").Get<Dictionary<string, string>>();
    builder.Services.SetupCors(corsSettings);

    // Add services to the container.
    builder.Services
        .AddControllers()
        .AddControllersAsServices();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services
        .AddEndpointsApiExplorer()
        .AddSwaggerGen(options => options.SetupSwagger())
        .AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

    // Using problem details
    // This middleware provides room for customization
    // https://code-maze.com/using-the-problemdetails-class-in-asp-net-core-web-api/
    // https://lurumad.github.io/problem-details-an-standard-way-for-specifying-errors-in-http-api-responses-asp.net-core
    builder.Services.AddProblemDetails();
    var app = builder.Build();

    // Print out internal admin password
    var internalAdmin = app.Services.GetRequiredService<InternalAdministrator>();
    Log.Information("Internal administrator credentials: {InternalAdmin} / {InternalAdminPassword}",
        internalAdmin.UserName,
        internalAdmin.Password);

    // Setup middleware pipeline
    app.UseSwagger()
        .UseSwaggerUI(options => options.SetupSwaggerUI())
        .UseProblemDetails()
        .UseCors(corsSettings)
        .UseHttpsRedirection()
        .UseMetricServer()
        .UseHttpMetrics()
        .UseSerilogRequestLogging();    

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapGet("database", async (context) =>
    {
        using var scope = context.RequestServices.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<SimpleSplitDbContext>();
        var sql = db.Database.GenerateCreateScript();
        await context.Response.WriteAsync(sql);
    });

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
{
    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
    return new ConfigurationBuilder()
        .SetBasePath(path)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
        .AddEnvironmentVariables()
        .Build();
}

// To support integration tests
public partial class Program { }