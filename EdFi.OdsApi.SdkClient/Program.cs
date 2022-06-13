using System;
using System.CommandLine;
using System.IO;
using System.Threading.Tasks;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Infrastructure;
using EdFi.AlmaToEdFi.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Amazon.Extensions.NETCore.Setup;
using Amazon;
using Amazon.Runtime;
using System.CommandLine.Invocation;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace EdFi.AlmaToEdFi.Cmd
{

    class Program
    {
        static int Main(string[] args)
        {
            var commandLineParameters = CommandLineParameters.GetCommandLineParameters();
            commandLineParameters.Handler = CommandHandler.Create<string, string, string, string, string, string>(async (awsSourceConnectionName, awsDestinationConnectionName, schoolYearFilter, SchoolFilter, awsRegion, awsLoggingGroupName) =>
            {
                // Trust all SSL certs -- needed unless signed SSL certificates are configured.
                // Trust all SSL certs -- needed unless signed SSL certificates are configured.
                System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                //Explicitly configures outgoing network calls to use the latest version of TLS where possible.
                //Due to our reliance on some older libraries, the.NET framework won't necessarily default
                //to the latest unless we explicitly request it. Some hosting environments will not allow older versions
                //of TLS, and thus calls can fail without this extra configuration.
                System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
                var config = GetConfiguration().Build();
              
                var settings = config.GetSection("Settings").Get<AppSettings>();
                settings = CommandLineParameters.CheckParameters(settings, awsSourceConnectionName, awsDestinationConnectionName, schoolYearFilter, SchoolFilter, awsRegion, awsLoggingGroupName);
                // Initialize the Service Collection
                var services = new ServiceCollection();
                IoCConfig.RegisterDependencies(services, config, settings);
                // Add Interceptors so Dynamic Proxy can do its work and intercept
                services.ConfigureDynamicProxy(config => config.Interceptors.AddTyped<CacheAttribute>());
                // Normally you build normally(with .Build). 
                //var serviceProvider = services.BuildDynamicProxyProvider();
                var serviceProvider = services.BuildDynamicProxyProvider();

                // Resolve and run the App
                await serviceProvider.GetService<App>().Run(args);

                Console.WriteLine();
                Console.WriteLine("Hit ANY key to continue...");
                Console.ReadLine();
            });
            // Parse the incoming args and invoke the handler
            try
            {
                return commandLineParameters.Invoke(args);
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write(e.Message);
                Console.ResetColor ();
                return -1;
            }
        }

        private static IConfigurationBuilder GetConfiguration()
        {
            var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);         
            // Switch Config Provider based on appSettings.json
            var settings = configBuilder.Build().GetSection("Settings").Get<AppSettings>();
            if (settings.AlmaAPI.ParameterStoreProvider.ToLower().Contains("awsparamstore") )
            {
                //configBuilder.AddSystemsManager("/AlmaApi/",
                //                                new AWSOptions
                //                                {
                //                                    Region = RegionEndpoint.GetBySystemName(awsRegion)
                //                                }, TimeSpan.FromSeconds(20));
                configBuilder.AddSystemsManager("/AlmaApi/", TimeSpan.FromSeconds(20));
            }
            return configBuilder;
        }

    }
}
