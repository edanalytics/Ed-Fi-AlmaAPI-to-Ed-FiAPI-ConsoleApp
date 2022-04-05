using System;
using System.IO;
using System.Threading.Tasks;
using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EdFi.AlmaToEdFi.Cmd
{

    class Program
    {
        static async Task Main(string[] args)
        {
            // Trust all SSL certs -- needed unless signed SSL certificates are configured.
            System.Net.ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

            //Explicitly configures outgoing network calls to use the latest version of TLS where possible.
            //Due to our reliance on some older libraries, the.NET framework won't necessarily default
            //to the latest unless we explicitly request it. Some hosting environments will not allow older versions
            //of TLS, and thus calls can fail without this extra configuration.
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            var jsonPath = Path.Combine(Environment.CurrentDirectory, "DescriptorMappings");
            // Create config
            var config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddMultipleJsonFiles(jsonPath)
                .Build();
            // Initialize the Service Collection
            var services = new ServiceCollection();
            IoCConfig.RegisterDependencies(services, config);
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
        }


    }
}
