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
            //This is attempted to be handeled inside of the constructor for student by checking if it's null. However, since we send it through Convert.ToDateTime the null value is converted to the default .NET value which is outside of the SQL range.
            if (String.IsNullOrEmpty(srcStudent.dob))
            {
                srcStudent.dob = "9999-12-30";
                Console.WriteLine($"INFO: Date of birth has been defaulted for StudentID: {srcStudent.id}. This is because the API is returning either a null or empty date of birth for this student.");
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
