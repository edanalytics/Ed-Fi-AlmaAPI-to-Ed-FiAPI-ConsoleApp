using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentSchoolAssociationsTransformer
    {
        EdFiStudentSchoolAssociation TransformSrcToEdFi(int schoolId, string schoolYearId, Enrollment srcEnrollment);
    }
    public class StudentSchoolAssociationsTransformer : IStudentSchoolAssociationsTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StudentSchoolAssociationsTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public EdFiStudentSchoolAssociation TransformSrcToEdFi(int schoolId, string schoolYearId, Enrollment srcEnrollment)
        {
            var schoolReference = new EdFiSchoolReference(schoolId, null);
            //Use a helper function to translate the almaID to a StateId.
            StudentTranslation st = new StudentTranslation();
            Student studentResponse = StudentTranslation.GetStudentById(srcEnrollment.studentId);
            EdFiStudentReference studentReference = null;
            //Check to see if the returned StateId is null. If it is then try using the AlmaStudentID.
            //This will not insert the student's information but an INFO has already been output for this student during the student posts.
            if (studentResponse.stateId != null)
            {
                studentReference = new EdFiStudentReference(studentResponse.stateId);
            }
            else
            {
                studentReference = new EdFiStudentReference(srcEnrollment.studentId);
            }

            int schoolYearEnd = int.Parse(StudentTranslation.GetSchoolYear().Substring(5, 4));
            EdFiSchoolYearTypeReference schoolYearTypeReference = new EdFiSchoolYearTypeReference(schoolYearEnd);

            GradeLevel gradeLevel = StudentTranslation.GetStudentGradeLevel(srcEnrollment.studentId, schoolYearId, srcEnrollment.school);
            if (gradeLevel.gradeLevelAbbr == null)
            {

            }


            //var studentReference = new EdFiStudentReference(srcEnrollment.studentId, null);
            return new EdFiStudentSchoolAssociation(null,Convert.ToDateTime(srcEnrollment.date) , null, null, null, schoolReference, schoolYearTypeReference, 
                studentReference, null, null, null,GetEdFiGradeLevelDescriptors(gradeLevel.gradeLevelAbbr));

        }

        public EdFiStudentSchoolAssociation TransformSrcToEdFi(int schoolId, Enrollment srcEnrollment)
        {
            throw new NotImplementedException();
        }

        private string  GetEdFiGradeLevelDescriptors(string  scrGradelevel)
        {
            return _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", scrGradelevel);
        }
    }
}
