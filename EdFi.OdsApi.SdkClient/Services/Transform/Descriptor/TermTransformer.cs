using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ITermTransformer
    {
        string TransformSrcToEdFi(string srcTermDescriptor);
    }
    public class TermTransformer : ITermTransformer
    {
        private readonly IDescriptorMapping _terms;
        public TermTransformer(IOptions<TermMapping> mapping)
        {
            _terms = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcTermDescriptor)
        {
            var map = _terms.Mapping.SingleOrDefault(x => x.Src == srcTermDescriptor);
            if (map == null)
                map = _terms.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
