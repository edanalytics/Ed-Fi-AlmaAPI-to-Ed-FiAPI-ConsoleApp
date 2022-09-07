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
            //Check to see if the Student DoB is null or empty. The code below will enter "1/1/0001" as the default value even when fed a null value.
            //This is an issue because SQL rejects anything before 01/01/1753.
            if (String.IsNullOrEmpty(srcStudent.dob))
            {
                srcStudent.dob = "9999-12-30";
            }
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
