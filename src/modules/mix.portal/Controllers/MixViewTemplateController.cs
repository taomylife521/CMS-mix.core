﻿using Microsoft.AspNetCore.Mvc;
using Mix.Auth.Constants;
using Mix.Lib.Interfaces;
using Mix.Mq.Lib.Models;
using Mix.SignalR.Interfaces;

namespace Mix.Portal.Controllers
{
    [Route("api/v2/rest/mix-portal/mix-view-template")]
    [ApiController]
    [MixAuthorize(MixRoles.Owner)]
    public class MixViewTemplateController : MixRestfulApiControllerBase<MixViewTemplateViewModel, MixCmsContext, MixTemplate, int>
    {
        public MixViewTemplateController(
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,
            MixCacheService cacheService,
            MixIdentityService mixIdentityService,
            UnitOfWorkInfo<MixCmsContext> uow,
            IMemoryQueueService<MessageQueueModel> queueService,
            IPortalHubClientService portalHub,
            IMixTenantService mixTenantService)
            : base(httpContextAccessor, configuration, cacheService, mixIdentityService, uow, queueService, portalHub, mixTenantService)
        {
        }
    }
}
