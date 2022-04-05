using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IAddressTypeTransformer
    {
        string TransformSrcToEdFi(string srcAddressType);
    }
    public class AddressTypeTransformer : IAddressTypeTransformer
    {
        private readonly IDescriptorMapping _addressType;
        public AddressTypeTransformer(IOptions<AddressTypeMapping> mapping)
        {
            _addressType = mapping.Value;
        }

        public string TransformSrcToEdFi(string srcAddressType)
        {
            var map = _addressType.Mapping.SingleOrDefault(x => x.Src == srcAddressType);
            if (map == null)
                map = _addressType.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
