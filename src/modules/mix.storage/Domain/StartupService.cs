﻿using Mix.Lib.Interfaces;
using Mix.Shared.Interfaces;
using Mix.Storage.Lib.Extensions;

namespace Mix.Storage.Domain
{
    public class StartupService : IStartupService
    {
        public void AddServices(IHostApplicationBuilder builder)
        {
            builder.Services.AddMixStorage();
        }

        public void UseApps(IApplicationBuilder app, IConfiguration configuration, bool isDevelop)
        {
        }

        public void UseEndpoints(IEndpointRouteBuilder endpoints, IConfiguration configuration, bool isDevelop)
        {
        }
    }
}
