﻿using Mix.Lib.Conventions;
using Mix.Lib.Providers;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGeneratedRestApi(this IServiceCollection services, List<Assembly> assemblies)
        {
            List<Type> restCandidates = GetCandidatesByAttributeType(
                assemblies, typeof(GenerateRestApiControllerAttribute));
            services.
                AddControllers(o => o.Conventions.Add(
                    new GenericControllerRouteConvention()
                )).
                ConfigureApplicationPartManager(m =>
                {
                    m.FeatureProviders.Add(
                        new GenericTypeControllerFeatureProvider(restCandidates));
                })
                .AddJsonOptions(opts =>
                {
                    var enumConverter = new JsonStringEnumConverter();
                    opts.JsonSerializerOptions.Converters.Add(enumConverter);
                })
                .AddNewtonsoftJson(options =>
                    options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter()));
            return services;
        }
    }
}