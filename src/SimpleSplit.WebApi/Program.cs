using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Autofac.Extensions.DependencyInjection;
using Hellang.Middleware.ProblemDetails;
using Microsoft.EntityFrameworkCore;
using Prometheus;
using Serilog;
using SimpleSplit.Application;
using SimpleSplit.Application.Features.Security;
using SimpleSplit.Infrastructure;
using SimpleSplit.Infrastructure.Persistence;
using SimpleSplit.WebApi.Swagger;
using Swashbuckle.AspNetCore.Filters;

// God knows why debug terminal starts minimized!
#if (DEBUG && WINDOWS)
if (Debugger.IsAttached)
{
    var p = Process.GetCurrentProcess();
    ShowWindow(p.MainWindowHandle, RESTORE);
}
#endif

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
    foreach (var corsKey in corsSettings.Keys)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(corsKey, policyBuilder =>
            {
                policyBuilder
                    .WithOrigins(corsSettings[corsKey])
                    .SetIsOriginAllowed(isOriginAllowed: _ => true)
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .AllowAnyMethod();
            });
        });
    }

    // Add services to the container.
    builder.Services
        .AddControllers()
        .AddControllersAsServices();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options => options.SetupSwagger());
    builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetEntryAssembly());

    // Using problem details
    // This middleware provides room for customization
    // https://code-maze.com/using-the-problemdetails-class-in-asp-net-core-web-api/
    // https://lurumad.github.io/problem-details-an-standard-way-for-specifying-errors-in-http-api-responses-asp.net-core
    builder.Services.AddProblemDetails();

    // Print out internal admin password
    var app = builder.Build();
    var internalAdmin = app.Services.GetRequiredService<InternalAdministrator>();
    Log.Information("Internal administrator credentials: {InternalAdmin} / {InternalAdminPassword}",
        internalAdmin.UserName,
        internalAdmin.Password);

    app.UseProblemDetails();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SetupSwaggerUI());
    }

    corsSettings
        .Keys
        .ToList()
        .ForEach(key => app.UseCors(key));
    
    app.UseHttpsRedirection();
    app.UseSerilogRequestLogging();

    app.MapControllers();
    app.MapGet("database", async (context) =>
    {
        using var scope = context.RequestServices.CreateScope();
        var db  = scope.ServiceProvider.GetRequiredService<SimpleSplitDbContext>();
        var sql = db.Database.GenerateCreateScript();
        await context.Response.WriteAsync(sql);
    });

    app.UseMetricServer();
    app.UseHttpMetrics();

    app.UseAuthentication();
    app.UseAuthorization();

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
public partial class Program
{
#if (DEBUG && WINDOWS)
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int cmdShow);
    private const int HIDE = 0;
    private const int MAXIMIZE = 3;
    private const int MINIMIZE = 6;
    private const int RESTORE = 9;
#endif
}