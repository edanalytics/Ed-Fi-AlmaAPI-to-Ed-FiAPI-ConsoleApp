using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ICourseTransformer
    {
        EdFiCourse TransformSrcToEdFi(int schoolId, Course srcCourse);
    }
    public class CourseTransformer : ICourseTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public CourseTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public EdFiCourse TransformSrcToEdFi(int schoolId, Course srcCourse)
        {
            var educationOrganizationReference = new EdFiEducationOrganizationReference(schoolId, null);
            var courseIdentificationCodes = GetEdFiCourseIdentificationCodeDescriptors(srcCourse.id);
            var courseCode = string.IsNullOrEmpty(srcCourse.code) ? srcCourse.id : srcCourse.code;
            return new EdFiCourse(null, courseCode, courseIdentificationCodes, educationOrganizationReference,
                                    GetEdFiAdemicSubjectDescriptors(srcCourse.Subjects.FirstOrDefault().name),
                                    null, null, null, null, null, srcCourse.name, null, null, null, null, null, null, null, null, null, null, null, null,1,
                                   GetEdFiGradeLevelDescriptors(srcCourse.GradeLevels), null, null);

        }
        private List<EdFiCourseOfferedGradeLevel> GetEdFiGradeLevelDescriptors(List<GradeLevel> almaGradeLevels)
        {
            var almaGl = almaGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key).ToList();//Get alma Grade Levels

            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", almaGl);
            var edfiGradeLevels = new List<EdFiCourseOfferedGradeLevel>();
            foreach (var agl in edfiStringDescriptors)
            {
                if (!edfiGradeLevels.Contains(new EdFiCourseOfferedGradeLevel(agl)))
                {
                    edfiGradeLevels.Add(new EdFiCourseOfferedGradeLevel(agl));
                }
                
            }
            return edfiGradeLevels;
        }
        public string GetEdFiAdemicSubjectDescriptors(string srcSubjectDescriptor)
        {
           return _descriptorMappingService.MappAlmaToEdFiDescriptor("AcademicSubjectDescriptor", srcSubjectDescriptor);
        }
        public List<EdFiCourseIdentificationCode> GetEdFiCourseIdentificationCodeDescriptors(string srcIdentificationCode)
        {
            var edfiDescriptors = new List<EdFiCourseIdentificationCode>();
            edfiDescriptors.Add(new EdFiCourseIdentificationCode(
                                _descriptorMappingService.MappAlmaToEdFiDescriptor("CourseIdentificationSystemDescriptor", "State course code"),
                                null, null, srcIdentificationCode));
            return edfiDescriptors;
        }
    }
}
