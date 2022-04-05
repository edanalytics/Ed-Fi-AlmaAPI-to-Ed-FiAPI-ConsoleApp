using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentsTransformer
    {
        EdFiStudent TransformSrcToEdFi(Student srcStudent);
    }

    public class StudentsTransformer : IStudentsTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StudentsTransformer(
            IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public EdFiStudent TransformSrcToEdFi(Student srcStudent)
        {
            return  new EdFiStudent(null, srcStudent.id, null, null, null, Convert.ToDateTime(srcStudent.dob), null, 
                                    GetEdFiGenderDescriptors(srcStudent.gender), null, null, null, srcStudent.firstName,
                                    null, null, srcStudent.lastName, null, srcStudent.middleName);
        }
        private string GetEdFiGenderDescriptors(string scrGender)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("SexDescriptor", scrGender);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("SexDescriptor", "default");
            return edfiStringDescriptors;
        }
    }
}
