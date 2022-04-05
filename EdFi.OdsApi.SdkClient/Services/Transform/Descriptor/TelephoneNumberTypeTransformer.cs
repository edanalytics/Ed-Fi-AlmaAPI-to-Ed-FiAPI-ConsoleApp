using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ITelephoneNumberTypeTransformer
    {
        string TransformSrcToEdFi(string srcTermDescriptor);
    }
    public class TelephoneNumberTypeTransformer : ITelephoneNumberTypeTransformer
    {
        private readonly IDescriptorMapping _telephoneNumberType;
        public TelephoneNumberTypeTransformer(IOptions<TelephoneNumberTypeMapping> mapping)
        {
            _telephoneNumberType = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcNumberTypeDescriptor)
        {
            var map = _telephoneNumberType.Mapping.SingleOrDefault(x => x.Src == srcNumberTypeDescriptor);
            if (map == null)
                map = _telephoneNumberType.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
