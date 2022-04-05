using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IClassroomPositionDescriptorTransformer
    {
        string TransformSrcToEdFi(string srcClassroomPosition);
    }
    public class ClassroomPositionDescriptorTransformer : IClassroomPositionDescriptorTransformer
    {
        private readonly IDescriptorMapping _classroomPosition;
        public ClassroomPositionDescriptorTransformer(IOptions<ClassroomPositionMapping> mapping)
        {
            _classroomPosition = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcClassroomPosition)
        {
            var map = _classroomPosition.Mapping.SingleOrDefault(x => x.Src == srcClassroomPosition);
            if (map == null)
                map = _classroomPosition.Mapping.SingleOrDefault(x => x.Src == "Teacher");
            return map.Dest;
        }
    }
}
