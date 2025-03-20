using BiopasIntegraVida.Infrastructure.Interfaces;
using BiopasIntegraVida.InterPlayers.Services;
using BiopasIntegraVida.Web.Configuration;
using Microsoft.AspNetCore.ResponseCompression;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

var appSettings = new AppSettingsValues(builder.Configuration);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var loggerConfig = new LoggerConfiguration();
loggerConfig = loggerConfig.WriteTo.Console();

if(!string.IsNullOrWhiteSpace(appSettings.SentryDsn))
{
    loggerConfig = loggerConfig.WriteTo.Sentry(o =>
    {
        o.MinimumBreadcrumbLevel = LogEventLevel.Information;
        o.MinimumEventLevel = LogEventLevel.Error;
        o.Dsn = appSettings.SentryDsn;
    });
}

Log.Logger = loggerConfig.CreateLogger();

builder.Services.AddSerilog();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<BrotliCompressionProvider>();
    options.Providers.Add<GzipCompressionProvider>();
});

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IInterPlayersConfig, InterPlayersConfig>();
builder.Services.AddSingleton<InterPlayersService>();
builder.Services.AddSingleton<AppSettingsValues>(appSettings);
builder.Services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

builder.Services.AddWebOptimizer(pipeline =>
{
    if (appSettings.ApplicationConfigCompressContent)
    {
        pipeline.MinifyCssFiles();
        pipeline.MinifyHtmlFiles();
        pipeline.MinifyJsFiles();
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (appSettings.ApplicationConfigCompressContent)
    app.UseResponseCompression();

app.UseWebOptimizer();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();