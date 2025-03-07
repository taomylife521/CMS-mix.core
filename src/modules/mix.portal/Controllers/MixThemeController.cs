﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mix.Database.Services;
using Mix.Database.Services.MixGlobalSettings;
using Mix.Heart.Constants;
using Mix.Lib.Interfaces;
using Mix.Mq.Lib.Models;
using Mix.Portal.Domain.Interfaces;
using Mix.Shared.Helpers;
using Mix.Shared.Models.Configurations;
using Mix.Shared.Services;
using Mix.SignalR.Constants;
using Mix.SignalR.Hubs;
using Mix.SignalR.Interfaces;
using System.Text.RegularExpressions;

namespace Mix.Portal.Controllers
{
    [Route("api/v2/rest/mix-portal/mix-theme")]
    [ApiController]
    [MixAuthorize(MixRoles.Owner)]
    public class MixThemeController
        : MixRestfulApiControllerBase<MixThemeViewModel, MixCmsContext, MixTheme, int>
    {
        protected readonly IHubContext<MixThemeHub> HubContext;
        private readonly HttpService _httpService;
        private readonly IMixThemeExportService _exportService;
        private readonly IMixThemeImportService _importService;
        private readonly MixConfigurationService _configService;
        private readonly AppSettingsService _appSettingsService;

        public MixThemeController(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            MixCacheService cacheService,
            MixIdentityService mixIdentityService,
            IMixThemeImportService importService,
            IMixThemeExportService exportService,
            UnitOfWorkInfo<MixCmsContext> cmsUow,
            IMemoryQueueService<MessageQueueModel> queueService,
            HttpService httpService,
            IHubContext<MixThemeHub> hubContext,
            MixConfigurationService configService,
            IPortalHubClientService portalHub,
            IMixTenantService mixTenantService,
            AppSettingsService appSettingsService)
            : base(httpContextAccessor, configuration,
                  cacheService, mixIdentityService, cmsUow, queueService, portalHub, mixTenantService)
        {

            _exportService = exportService;
            _importService = importService;
            _httpService = httpService;
            HubContext = hubContext;
            _configService = configService;
            _appSettingsService = appSettingsService;
        }

        #region Routes

        // POST api/theme
        [HttpPost]
        [Route("save")]
        public async Task<ActionResult<MixThemeViewModel>> Save(MixThemeViewModel data)
        {
            data.SetUowInfo(Uow, CacheService);
            await data.SaveAsync();
            return Ok(data);
        }

        [HttpPost("export")]
        public async Task<ActionResult<SiteDataViewModel>> ExportThemeAsync([FromBody] ExportThemeDto dto, CancellationToken cancellationToken = default)
        {
            var siteData = await _exportService.ExportTheme(dto, cancellationToken);
            return Ok(siteData);
        }

        [HttpPost("load-theme")]
        public async Task<ActionResult<SiteDataViewModel>> LoadThemeAsync(IFormFile theme)
        {
            _importService.ExtractTheme(theme);
            var siteData = await _importService.LoadSchema();
            return Ok(siteData);
        }

        [HttpPost("import-theme")]
        public async Task<ActionResult<SiteDataViewModel>> ImportThemeAsync([FromBody] ImportThemeDto dto, CancellationToken cancellationToken = default)
        {
            if (ModelState.IsValid)
            {
                var siteData = await _importService.LoadSchema();
                siteData.ThemeId = dto.ThemeId;
                siteData.MixDatabases = siteData.MixDatabases.Where(m => dto.MixDatabaseIds.Contains(m.Id)).ToList();
                siteData.MixDatabaseColumns = siteData.MixDatabaseColumns.Where(m => dto.MixDatabaseIds.Contains(m.MixDatabaseId)).ToList();
                siteData.MixDatabaseRelationships = siteData.MixDatabaseRelationships.Where(
                    m => siteData.MixDatabases.Any(n => n.SystemName == m.SourceDatabaseName)
                            && siteData.MixDatabases.Any(n => n.SystemName == m.DestinateDatabaseName)).ToList();

                var exportDataDbNames = siteData.MixDatabases.Where(m => dto.MixDatabaseDataIds.Contains(m.Id));
                siteData.MixDbModels = siteData.MixDbModels.Where(m => exportDataDbNames.Any(n => n.SystemName == m.DatabaseName)).ToList();

                siteData.CreatedBy = MixIdentityService.GetClaim(User, MixClaims.UserName);
                siteData.Specificulture ??= CurrentTenant.Configurations.DefaultCulture;
                var result = await _importService.ImportSelectedItemsAsync(siteData, siteData.CreatedBy, cancellationToken);
                return Ok(result);
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        [Route("install")]
        public async Task<ActionResult<bool>> InstallTheme([FromBody] JObject theme)
        {
            var progress = new Progress<int>();
            var percent = 0;
            progress.ProgressChanged += (sender, value) =>
            {
                if (value > percent)
                {
                    percent = value;
                    _ = AlertAsync(HubContext.Clients.Group("Theme"), "Downloading", 200, value);

                }
            };

            await _importService.DownloadThemeAsync(theme, progress, _httpService);
            _appSettingsService.SetConfig(nameof(AppSettingsModel.InitStatus), InitStep.SelectTheme);
            return Ok();
        }

        [HttpPost]
        [Route("install-portal")]
        public async Task<ActionResult<string>> InstallPortal([FromBody] JObject theme, [FromServices] IThemeService themeService)
        {
            var progress = new Progress<int>();
            var percent = 0;
            progress.ProgressChanged += (sender, value) =>
            {
                if (value > percent)
                {
                    percent = value;
                    _ = AlertAsync(HubContext.Clients.Group("Theme"), "Downloading", 200, value);

                }
            };

            string name = SeoHelper.GetSEOString(theme.Value<string>("title"));
            string folder = $"{MixFolders.WebRootPath}/{MixFolders.PortalApps}/{name}";
            string appFolder = $"{MixFolders.PortalApps}/{name}";
            await _importService.DownloadThemeAsync(theme, progress, _httpService, folder);
            Thread.Sleep(300);
            var indexFile = MixFileHelper.GetFileByFullName($"{folder}/index.html");
            Regex regex = new("((?<=src=\")|(?<=href=\"))(?!(http[^\\s]+))(.+?)(\\.+?)");

            if (indexFile.Content != null && regex.IsMatch(indexFile.Content))
            {
                indexFile.Content = regex.Replace(indexFile.Content, $"/{appFolder}/$3$4")
                    .Replace("options['baseHref']", $"'{appFolder}'");

                var activeTheme = await themeService.GetActiveTheme();
                MixTemplateViewModel template = new(Uow)
                {
                    MixThemeId = activeTheme.Id,
                    FileName = name,
                    FileFolder = $"{MixFolders.TemplatesFolder}/{CurrentTenant.SystemName}/{activeTheme.SystemName}/{MixTemplateFolderType.Pages}",
                    FolderType = MixTemplateFolderType.Pages,
                    Extension = MixFileExtensions.CsHtml,
                    Content = "@{Layout = null;}" + indexFile.Content.Replace("@", "@@"),
                    TenantId = CurrentTenant.Id,
                    Scripts = string.Empty,
                    Styles = string.Empty
                };
                await template.SaveAsync();
            }

            MixFileHelper.SaveFile(indexFile);
            return Ok(appFolder);
        }

        #endregion

        #region Overrides

        protected override Task<int> CreateHandlerAsync(MixThemeViewModel data, CancellationToken cancellationToken = default)
        {
            data.AssetFolder = $"{MixFolders.StaticFiles}/{CurrentTenant.SystemName}/{data.SystemName}";
            data.TemplateFolder = $"{MixFolders.TemplatesFolder}/{CurrentTenant.SystemName}/{data.SystemName}";
            return base.CreateHandlerAsync(data, cancellationToken);
        }

        protected override Task UpdateHandler(int id, MixThemeViewModel data, CancellationToken cancellationToken = default)
        {
            data.AssetFolder = $"{MixFolders.StaticFiles}/{CurrentTenant.SystemName}/{data.SystemName}";
            data.TemplateFolder = $"{MixFolders.TemplatesFolder}/{CurrentTenant.SystemName}/{data.SystemName}";
            return base.UpdateHandler(id, data, cancellationToken);
        }

        protected override PagingResponseModel<MixThemeViewModel> ParseSearchResult(SearchRequestDto req, PagingResponseModel<MixThemeViewModel> result)
        {
            var data = base.ParseSearchResult(req, result);
            var activeId = _configService.GetConfig<int?>(MixConfigurationNames.ActiveThemeId, Culture.Specificulture, CurrentTenant.Id).GetAwaiter().GetResult();
            if (activeId.HasValue)
            {
                foreach (var item in data.Items)
                {
                    if (activeId == item.Id)
                    {
                        item.IsActive = true;
                        break;
                    }
                }
            }
            return data;
        }

        #endregion

        #region Helpers
        public virtual async Task AlertAsync<T>(IClientProxy clients, string action, int status, T message)
        {
            var address = Request.Headers["X-Forwarded-For"];
            if (string.IsNullOrEmpty(address))
            {
                address = Request.Host.Value;
            }
            var logMsg = new JObject()
                {
                    new JProperty("created_at", DateTime.UtcNow),
                    new JProperty("id", Request.HttpContext.Connection.Id.ToString()),
                    new JProperty("address", address),
                    new JProperty("ip_address", Request.HttpContext.Connection.RemoteIpAddress.ToString()),
                    new JProperty("user", MixIdentityService.GetClaim(User, MixClaims.UserName)),
                    new JProperty("request_url", Request.Path.Value),
                    new JProperty("action", action),
                    new JProperty("status", status),
                    new JProperty("message", message)
                };

            //It's not possible to configure JSON serialization in the JavaScript client at this time (March 25th 2020).
            //https://docs.microsoft.com/en-us/aspnet/core/signalr/configuration?view=aspnetcore-3.1&tabs=dotnet
            await clients.SendAsync(
                HubMethods.ReceiveMethod, logMsg.ToString(Formatting.None));
        }
        #endregion
    }
}
