using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Extractors.Alma;
using EdFi.AlmaToEdFi.Cmd.Services.Processors;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Linq;
using EdFi.AlmaToEdFi.Common;
using EdFi.AlmaToEdFi.Cmd.Services;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.OdsApi.Sdk.Client;
using System;
using EdFi.AlmaToEdFi.Cmd.Model;
using System.IO;
using Amazon.Runtime;
using System.Collections.Generic;
using System.Text.Json;

namespace EdFi.AlmaToEdFi.Cmd.Infrastructure
{
    public static class IoCConfig
    {
        public static void RegisterDependencies(IServiceCollection container, IConfiguration config, AppSettings appSettings)
        {
            Alma.Api.Sdk.Infrastructure.IoCConfig.RegisterDependencies(container, config);
            RegisterApplicationDependencies(container, config, appSettings);
            RegisterExtractorsByConvention<ISchoolYearsExtractor>(container);
            RegisterTransformersByConvention<IEducationOrganizationAddressTransformer>(container);
            RegisterProcessorsByConvention<IProcessor>(container);

            // Once everything is configured then lets configure the App.
            container.AddTransient<App>();
            container.AddTransient<AppEdfitest>();
        }


        private static void RegisterApplicationDependencies(IServiceCollection container, IConfiguration config, AppSettings appSettings)
        {
            // Configure the Logger
            container.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
                //check if the log going for CloudWatch or File
                if (appSettings.Logging.LoggingProvider.ToLower().Contains("awscloudwatch"))
                {
                    LoggerFactory logFactory = new LoggerFactory();
                    var configLog = new AWS.Logger.AWSLoggerConfig(appSettings.Logging.LogGroup== "" ? "AlmaApi" : appSettings.Logging.LogGroup);
                    configLog.Region = appSettings.Logging.Region;
                    configLog.LogStreamNamePrefix = appSettings.Logging.LogStreamNamePrefix;
                   //configLog.Credentials = new BasicAWSCredentials(appSettings.AwsConfiguration.AWSAccessKey, appSettings.AwsConfiguration.AWSSecretKey);
                    builder.AddAWSProvider(configLog);
                }
                else 
                {
                    string logPath = Path.Combine(Environment.CurrentDirectory, "Log", $"AlmaAppLog{DateTime.Now.ToString("yyyy-MM-dd")}.log");
                    builder.AddFile(logPath, true);
                }
            });

            //SystemsManagerConfigurationProvider
            if (!string.IsNullOrEmpty(appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter))
                // if SchoolYearFilter comes from comand line parameter we overwrite the value
                config.GetSection("Settings:AlmaAPI:Connections:Alma:SourceConnection:SchoolYearFilter").Value =
                appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter;

            if (!string.IsNullOrEmpty(appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolFilter))
                // if SchoolFilter comes from comand line parameter we overwrite the value
                config.GetSection("Settings:AlmaAPI:Connections:Alma:SourceConnection:SchoolFilter").Value =
                appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolFilter;

            // Check if the app is enabled to AWS Parameter Store
            if (appSettings.AlmaAPI.ParameterStoreProvider.ToLower().Contains("awsparamstore"))
            {
                config.AddAlmaCustomParameters(appSettings.AlmaAPI.Connections.SourceConnectionFilter, appSettings.AlmaAPI.Connections.TargetConnectionFilter);
            }           

            // Register the IOptions for app settings.
            container.Configure<AppSettings>(config.GetSection("Settings"));
            // Exception Handler
            container.AddScoped<ILoadExceptionHandler, LoadExceptionHandler>();

            // Other manual dependencies to be configured...
            container.AddScoped<IAlmaRestClientConfigurationProvider, AlmaRestClientConfigurationProvider>();
            container.AddScoped<IAlmaApi, AlmaApi>();
            container.AddScoped<IEdFiApi, EdFiApi>();
            container.AddScoped<IAlmaLog, AlmaLog>();
            container.AddScoped<IServiceCollection, ServiceCollection>();

            container.AddSingleton<IDescriptorMappingService>(dMapping => new DescriptorMappingService());
        }
         
        private static void RegisterMappingsByConvention<TMarker>(IServiceCollection container, IConfiguration config)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes.Where(t=>t.GetInterface(nameof(IDescriptorMapping))!=null);

            var mappingsToRegister = from mappingType in types select mappingType;

            //foreach (var mapping in mappingsToRegister)
            //    container.Configure(;
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
                };

            foreach (var pair in transformersToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);
        }

        private static void RegisterTransformersByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var transformersToRegister =
                from interfaceType in types.Where(t => t.Name.StartsWith("I") && t.Name.EndsWith("Transformer"))
                from serviceType in types.Where(t => t.Name == interfaceType.Name.Substring(1))
                select new
                {
                    InterfaceType = interfaceType,
                    ServiceType = serviceType
                };

            foreach (var pair in transformersToRegister)
                container.AddScoped(pair.InterfaceType, pair.ServiceType);

           
        }

        private static void RegisterProcessorsByConvention<TMarker>(IServiceCollection container)
        {
            var types = typeof(TMarker).Assembly.ExportedTypes;

            var processors = types.Where(x => !x.IsAbstract && x.IsClass && x.GetInterface(nameof(IProcessor)) == typeof(IProcessor));

            foreach (var pType in processors)
                container.AddScoped(typeof(IProcessor), pType);
        }
    }
}
