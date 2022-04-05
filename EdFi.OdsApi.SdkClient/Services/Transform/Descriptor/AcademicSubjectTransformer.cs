using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IAcademicSubjectTransformer
    {
        string  TransformSrcToEdFi(string srcSubjectDescriptor);
    }
    public class AcademicSubjectTransformer : IAcademicSubjectTransformer
    {
        private readonly IDescriptorMapping _academicSubject;
        public AcademicSubjectTransformer(IOptions<AcademicSubjectMapping> mapping)
        {
            _academicSubject = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcSubjectDescriptor)
        {
             var descriptor = String.Empty;
            var map = _academicSubject.Mapping.SingleOrDefault(x => x.Src == srcSubjectDescriptor);
            if (map == null)
                map = _academicSubject.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
