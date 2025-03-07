using Microsoft.Extensions.DependencyInjection.Extensions;
using Mix.Database.Entities.Account;
using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using Mix.Log.Lib;
using Mix.Lib.Middlewares;
using Mix.Shared.Models.Configurations;
using Mix.Queue.Extensions;
using Mix.Lib.Services;
using Mix.Lib.Publishers;
using Mix.Lib.Subscribers;
using Mix.Quartz.Services;
using Mix.Storage.Lib.Subscribers;
using Mix.Log.Lib.Publishers;
using Mix.Log.Lib.Interfaces;
using Mix.Log.Lib.Services;
using Mix.Log.Lib.Models;
using Mix.Shared.Extensions;
using Mix.Mixdb.Publishers;
using Mix.Mixdb.Subscribers;
using Mix.Lib.Extensions;

var builder = MixCmsHelper.CreateWebApplicationBuilder(args);

builder.AddServiceDefaults();
// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddWebEncoders(options =>
{
    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
});
builder.Services.AddEndpointsApiExplorer();
builder.AddConfigurations();
var globalConfig = builder.Configuration.GetSection(MixAppSettingsSection.GlobalSettings)
                                            .Get<GlobalSettingsModel>();
builder.AddMixQueue();
builder.AddMixServices(Assembly.GetExecutingAssembly());
builder.AddMixLogPublisher();
builder.Services.ApplyMigrations(builder.Configuration, globalConfig);
builder.AddMixCors();
builder.Services.AddScoped<MixNavigationService>();
builder.Services.AddMixLogSubscriber(builder.Configuration);
builder.Services.AddMixAuthorize<MixCmsAccountContext>(builder.Configuration);
builder.Services.AddScoped<MixIdentityService>();
builder.Services.TryAddScoped<MixcorePostService>();

AddPortalServices(builder.Services, builder.Configuration);
var app = builder.Build();

app.MapDefaultEndpoints();

Configure(app, builder.Environment, builder.Configuration);

app.Run();


void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
{
    if (!env.IsLocal())
    {
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    app.UseHttpsRedirection();
    app.UseMixTenant();
    app.UseRouting();

    // Typically, UseStaticFiles is called before UseCors. Apps that use JavaScript to retrieve static files cross site must call UseCors before UseStaticFiles.
    app.UseMixStaticFiles(env.ContentRootPath);


    // UseCors must be placed after UseRouting and before UseAuthorization. This is to ensure that CORS headers are included in the response for both authorized and unauthorized calls.
    app.UseMixCors(configuration);

    // must go between app.UseRouting() and app.UseEndpoints.
    app.UseMixAuth();

    // auditlog middleware must go after auth
    app.UseMiddleware<AuditlogMiddleware>();
    app.UseMixRateLimiter();
    app.UseMixApps(Assembly.GetExecutingAssembly(), configuration, !env.IsProduction());
    app.UseMixSwaggerApps(!env.IsProduction(), Assembly.GetExecutingAssembly());
    app.UseResponseCompression();
    app.UseMixResponseCaching();
}

void AddPortalServices(IServiceCollection services, IConfiguration configuration)
{
    var globalConfigs = configuration.GetSection(MixAppSettingsSection.GlobalSettings).Get<GlobalSettingsModel>()!;
    services.AddMixRoutes();

    services.AddHostedService<MixRepoDbPublisher>();
    services.AddHostedService<MixRepoDbSubscriber>();
    services.AddHostedService<MixViewModelChangedPublisher>();
    services.AddHostedService<MixViewModelChangedSubscriber>();

    services.AddHostedService<MixBackgroundTaskPublisher>();
    services.AddHostedService<MixBackgroundTaskSubscriber>();
    services.AddHostedService<MixDbCommandPublisher>();
    services.AddHostedService<MixDbCommandSubscriber>();

    services.AddHostedService<MixQuartzHostedService>();
    services.AddHostedService<StorageBackgroundTaskSubscriber>();

    if (!configuration.IsInit())
    {
        services.TryAddSingleton<IAuditLogService, AuditLogService>();
        services.TryAddScoped<AuditLogDataModel>();
        services.AddHostedService<MixLogPublisher>();
    }

}
