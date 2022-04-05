using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{

    public interface IStaffClassificationTransformer
    {
        string TransformSrcToEdFi(string srcProgramAssigment);
    }
    public class StaffClassificationTransformer : IStaffClassificationTransformer
    {
        private readonly IDescriptorMapping _staffClassification;
        public StaffClassificationTransformer(IOptions<StaffClassificationMapping> mapping)
        {
            _staffClassification = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcProgramAssigment)
        {
            var map = _staffClassification.Mapping.SingleOrDefault(x => x.Src == srcProgramAssigment);
            if (map == null)
                map = _staffClassification.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
