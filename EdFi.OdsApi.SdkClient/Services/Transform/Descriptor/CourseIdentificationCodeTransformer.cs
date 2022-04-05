using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ICourseIdentificationCodeTransformer
    {
        List<EdFiCourseIdentificationCode> TransformSrcToEdFi(string srcIdentificationCode);
    }
    public class CourseIdentificationCodeTransformer : ICourseIdentificationCodeTransformer
    {
        private readonly IDescriptorMapping _courseIdentification;
        public CourseIdentificationCodeTransformer(IOptions<CourseIdentificationSystemMapping> mapping)
        {
            _courseIdentification = mapping.Value;
        }


        public List<EdFiCourseIdentificationCode> TransformSrcToEdFi(string srcIdentificationCode)
        {
            var edfiDescriptors = new List<EdFiCourseIdentificationCode>();
            edfiDescriptors.Add(new EdFiCourseIdentificationCode(_courseIdentification.Mapping.SingleOrDefault().Dest, null, null, srcIdentificationCode));
            return edfiDescriptors;
        }
    }
}
