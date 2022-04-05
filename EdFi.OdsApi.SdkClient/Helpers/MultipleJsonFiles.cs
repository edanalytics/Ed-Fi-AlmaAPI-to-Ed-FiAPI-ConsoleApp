using Microsoft.Extensions.Configuration;
namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
    public static class MultipleJsonFiles
    {
        public static IConfigurationBuilder AddMultipleJsonFiles(this IConfigurationBuilder configurationBuilder, string path)
        {
            string[] files = System.IO.Directory.GetFiles(path, "*.json");
            foreach (var item in files)
            {
                configurationBuilder.AddJsonFile(item, optional: false, reloadOnChange: true);
            }
            return configurationBuilder;
        }
    }
}
