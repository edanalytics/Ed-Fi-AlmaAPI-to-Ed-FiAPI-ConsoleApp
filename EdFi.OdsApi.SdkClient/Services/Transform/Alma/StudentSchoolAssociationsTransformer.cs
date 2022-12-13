using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentSchoolAssociationsTransformer
    {
        List<EdFiStudentSchoolAssociation> TransformSrcToEdFi(int schoolId, Enrollment srcEnrollment, StudentsGradeLevels studentGradeLevels, List<GradeLevel> gradeLevels);
        List<EdFiStudentSchoolAssociation> TransformSrcToEdFi(int schoolId, StudentsGradeLevels studentGradeLevels, List<GradeLevel> gradeLevels);
    }
    public class StudentSchoolAssociationsTransformer : IStudentSchoolAssociationsTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StudentSchoolAssociationsTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public List<EdFiStudentSchoolAssociation> TransformSrcToEdFi(int schoolId, StudentsGradeLevels studentGradeLevels, List<GradeLevel> gradeLevels)
        {
            var edFiStudentSchoolAssociations = new List<EdFiStudentSchoolAssociation>();
            var schoolReference = new EdFiSchoolReference(schoolId, null);
            foreach (var student in studentGradeLevels.students)
            {

                foreach (var gradeLevelItem in student.GradeLevels)
                {
                    //Use a helper function to translate the almaID to a StateId.
                    StudentTranslation st = new StudentTranslation();
                    Student studentResponse = StudentTranslation.GetStudentById(student.id);
                    EdFiStudentReference studentReference = null;
                    //Check to see if the returned StateId is null. If it is then try using the AlmaStudentID.
                    //This will not insert the student's information but an INFO has already been output for this student during the student posts.
                    if (studentResponse.stateId != null)
                    {
                        studentReference = new EdFiStudentReference(studentResponse.stateId);
                    }
                    else
                    {
                        studentReference = new EdFiStudentReference(studentResponse.id);
                    }
                    var gradeEnrollment = gradeLevels.Where(gl => gl.id == gradeLevelItem.gradeLevelId && gl.schoolYearId == gradeLevelItem.schoolYearId).ToList();
                    if (gradeEnrollment.Count > 0)
                        edFiStudentSchoolAssociations.Add(new EdFiStudentSchoolAssociation(null, Convert.ToDateTime(gradeEnrollment.SingleOrDefault().effectiveDate), null, null, null, schoolReference, null,
                                studentReference, null, null, null, GetEdFiGradeLevelDescriptors(gradeEnrollment.SingleOrDefault().gradeLevelAbbr)));
                }
            }

            return edFiStudentSchoolAssociations;
        }
        public List<EdFiStudentSchoolAssociation> TransformSrcToEdFi(int schoolId, Enrollment srcEnrollment, StudentsGradeLevels studentGradeLevels, List<GradeLevel> gradeLevels)
        {
            var edFiStudentSchoolAssociations = new List<EdFiStudentSchoolAssociation>();
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
            var studentGradeLevelEnrollment = studentGradeLevels.students.FirstOrDefault(x => x.id == srcEnrollment.studentId).GradeLevels;
            foreach (var gradeLevel in studentGradeLevelEnrollment)
            {
                var gradeEnrollment = gradeLevels.Where(gl => gl.id == gradeLevel.gradeLevelId && gl.schoolYearId == gradeLevel.schoolYearId).ToList();
                if (gradeEnrollment.Count > 0)
                    edFiStudentSchoolAssociations.Add(new EdFiStudentSchoolAssociation(null, Convert.ToDateTime(srcEnrollment.date), null, null, null, schoolReference, null,
                            studentReference, null, null, null, GetEdFiGradeLevelDescriptors(gradeEnrollment.SingleOrDefault().gradeLevelAbbr)));
            }

            return edFiStudentSchoolAssociations;
        }
        private string GetEdFiGradeLevelDescriptors(string scrGradelevel)
        {
            return _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", scrGradelevel);
        }
    }
}
