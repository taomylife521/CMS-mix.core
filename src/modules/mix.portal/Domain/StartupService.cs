﻿using Mix.Lib.Interfaces;
using Mix.Portal.Domain.Interfaces;
using Mix.Portal.Domain.Services;
using Mix.Shared.Interfaces;

namespace Mix.Portal.Domain
{
    public class StartupService : IStartupService
    {
        public void AddServices(IHostApplicationBuilder builder)
        {
            builder.Services.AddScoped<ICloneCultureService, CloneCultureService>();
            builder.Services.AddScoped<PortalPostService>();
        }

        public void UseApps(IApplicationBuilder app, IConfiguration configuration, bool isDevelop)
        {
        }

        public void UseEndpoints(IEndpointRouteBuilder endpoints, IConfiguration configuration, bool isDevelop)
        {
        }
    }
}
