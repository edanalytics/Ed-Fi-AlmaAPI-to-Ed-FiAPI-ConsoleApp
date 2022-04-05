using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IDescriptorMappingService
    {
        string MappAlmaToEdFiDescriptor(string descriptorNamespace, string almaCode);
        List<string> MappAlmaToEdFiDescriptor(string descriptorNamespace, List<string> almaCodes);
    }

    public class DescriptorMappingService : IDescriptorMappingService
    {
        private List<DescriptorMapping> _descriptorMappings = new List<DescriptorMapping>();

        // This is registered in IoC as singleton. We will not load all files every single time.
        public DescriptorMappingService()
        {
            var jsonPath = Path.Combine(Environment.CurrentDirectory, "DescriptorMappings");
            string[] jsonFiles = Directory.GetFiles(jsonPath, "*.json");
            jsonFiles.ToList().ForEach(file =>
            {
                var descriptorMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<DescriptorMapping>
                                                                                    (File.ReadAllText(file));
                _descriptorMappings.Add(descriptorMapping);
            });
        }

        public string MappAlmaToEdFiDescriptor(string descriptorNamespace, string almaCode)
        {
            // Filter out by the descriptors that we are going to use in this scope.
            var descriptors = _descriptorMappings.SingleOrDefault(d => d.Descriptor == descriptorNamespace);

            var map = descriptors.Mapping.SingleOrDefault(m => m.Src == almaCode);
            if (map == null)
                return null;
            return map.Dest;
        }

        public List<string> MappAlmaToEdFiDescriptor(string descriptorNamespace, List<string> almaCodes)
        {
            // Filter out by the descriptors that we are going to use in this scope.
            var descriptors = _descriptorMappings.SingleOrDefault(d => d.Descriptor == descriptorNamespace);

            var maps = new List<string>();

            foreach (var almaCode in almaCodes)
                maps.Add(descriptors.Mapping.SingleOrDefault(m => m.Src == almaCode).Dest);

            return maps;
        }
    }

    public class DescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class Mapping
    {
        public string Src { get; set; }
        public string Dest { get; set; }
    }
}
