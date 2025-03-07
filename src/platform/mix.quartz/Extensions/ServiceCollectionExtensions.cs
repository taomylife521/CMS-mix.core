﻿using Microsoft.Extensions.Configuration;
using Mix.Quartz.Interfaces;
using Mix.Quartz.Services;
using Mix.Shared;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IHostApplicationBuilder AddMixQuartzServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddSchedulerJobs();
            builder.Services.TryAddSingleton<IJobFactory, SingletonJobFactory>();
            builder.Services.TryAddSingleton<IQuartzService, QuartzService>();
            return builder;
        }

        private static void AddSchedulerJobs(this IServiceCollection services)
        {
            var assemblies = MixAssemblyFinder.GetAssembliesByPrefix("mix");

            foreach (var assembly in assemblies)
            {
                var mixJobs = assembly
                    .GetExportedTypes()
                    .Where(m => m.BaseType == typeof(MixJobBase));

                var method = typeof(ServiceCollectionServiceExtensions)
                    .GetMethods()
                    .First(m => m.IsGenericMethodDefinition
                        && m.Name == nameof(ServiceCollectionServiceExtensions.AddSingleton)
                        && m.GetGenericArguments().Length == 2);

                foreach (var job in mixJobs)
                {
                    MethodInfo generic = method.MakeGenericMethod(typeof(MixJobBase), job);
                    generic.Invoke(null, new object[] { services });
                }
            }
        }

    }
}