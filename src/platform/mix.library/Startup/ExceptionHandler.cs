﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ExceptionMiddlewareExtensions
    {
        // Handled in HttpResponseExceptionFilter.cs => not use

        public static void ConfigureExceptionHandler(this IApplicationBuilder app)
        {
            //app.UseDeveloperExceptionPage();
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async (context) =>
                {
                    var exception = context.Features
                   .Get<IExceptionHandlerPathFeature>()
                   .Error;

                    var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    if (contextFeature != null)
                    {
                        throw new MixException(MixErrorStatus.ServerError, contextFeature.Error);
                    }
                    await MixLogService.LogExceptionAsync(contextFeature.Error);
                });
            });
        }
    }
}
