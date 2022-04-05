using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IProgramAssignmentTransformer
    {
        string TransformSrcToEdFi(string srcProgramAssigment);
    }
    public class ProgramAssignmentTransformer : IProgramAssignmentTransformer
    {
        private readonly IDescriptorMapping _programAssignment;
        public ProgramAssignmentTransformer(IOptions<ProgramAssignmentMapping> mapping)
        {
            _programAssignment = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcProgramAssigment)
        {
            var map = _programAssignment.Mapping.SingleOrDefault(x => x.Src == srcProgramAssigment);
            if (map == null)
                map = _programAssignment.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
