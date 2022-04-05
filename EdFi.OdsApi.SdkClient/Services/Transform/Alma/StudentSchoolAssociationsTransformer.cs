using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentSchoolAssociationsTransformer
    {
        EdFiStudentSchoolAssociation TransformSrcToEdFi(int schoolId, Enrollment srcEnrollment);
    }
    public class StudentSchoolAssociationsTransformer : IStudentSchoolAssociationsTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StudentSchoolAssociationsTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public EdFiStudentSchoolAssociation TransformSrcToEdFi(int schoolId, Enrollment srcEnrollment)
        {
            var schoolReference = new EdFiSchoolReference(schoolId, null);
            var studentReference = new EdFiStudentReference(srcEnrollment.studentId, null);
            return new EdFiStudentSchoolAssociation(null,Convert.ToDateTime(srcEnrollment.date) , null, null, null, schoolReference, null, 
                studentReference, null, null, null,GetEdFiGradeLevelDescriptors("default"));

        }
        private string  GetEdFiGradeLevelDescriptors(string  scrGradelevel)
        {
            return _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", scrGradelevel);
        }
    }
}
