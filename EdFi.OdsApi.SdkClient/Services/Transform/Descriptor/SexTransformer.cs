using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ISexTransformer
    {
        string TransformSrcToEdFi(string srcSexDescriptor);
    }
    public class SexTransformer : ISexTransformer
    {
        private readonly IDescriptorMapping _sex;
        public SexTransformer(IOptions<SexMapping> mapping)
        {
            _sex = mapping.Value;
        }

        public string TransformSrcToEdFi(string srcSexDescriptor)
        {
            var map = _sex.Mapping.SingleOrDefault(x => x.Src == srcSexDescriptor);
            if (map == null)
                map = _sex.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
