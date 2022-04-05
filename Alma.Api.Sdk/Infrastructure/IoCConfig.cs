using Alma.Api.Sdk.Extractors.Alma;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Alma.Api.Sdk.Infrastructure
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration config)
        {
            RegisterApplicationDependencies(container, config);
            RegisterExtractorsByConvention<IAlmaApi>(container);
        }

        private static void RegisterApplicationDependencies(IServiceCollection container, IConfiguration config)
        {
            container.AddScoped<IAlmaRestClientConfigurationProvider, AlmaRestClientConfigurationProvider>();
            container.AddScoped<IAlmaApi, AlmaApi>();
        }

        private static void RegisterExtractorsByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var transformersToRegister =
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Extractor"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                }
            ;

            foreach (var pair in transformersToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }
    }
}
