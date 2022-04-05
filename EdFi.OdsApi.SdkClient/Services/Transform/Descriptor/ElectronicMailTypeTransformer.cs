using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IElectronicMailTypeTransformer
    {
        string TransformSrcToEdFi(string srcMailType);
    }
    public class ElectronicMailTypeTransformer : IElectronicMailTypeTransformer
    {
        private readonly IDescriptorMapping _electronicMail;
        public ElectronicMailTypeTransformer(IOptions<ElectronicMailTypeMapping> mapping)
        {
            _electronicMail = mapping.Value;
        }
        public string  TransformSrcToEdFi(string srcMailType)
        {
            var map = _electronicMail.Mapping.SingleOrDefault(x => x.Src == srcMailType);
            if (map == null)
                map = _electronicMail.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
