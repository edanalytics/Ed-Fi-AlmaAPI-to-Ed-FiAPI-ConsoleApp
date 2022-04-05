using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ICourseOfferingTransformer
    {
        EdFiCourseOffering TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions);
    }
    public class CourseOfferingTransformer : ICourseOfferingTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer; 
        private readonly IDescriptorMappingService _descriptorMappingService;
        

        public CourseOfferingTransformer(
            ISessionNameTransformer sessionNameTransformer,
            IDescriptorMappingService descriptorMappingService)
        {
            _sessionNameTransformer = sessionNameTransformer;
            _descriptorMappingService = descriptorMappingService;
        }

        
        public EdFiCourseOffering TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions)
        {
            var sessionName = _sessionNameTransformer.TransformSrcToEdFi( almaSessions, srcCourse.schoolYearId, srcCourse.effectiveDate);
            var courseCode = string.IsNullOrEmpty(srcCourse.code) ? srcCourse.id : srcCourse.code;
            var courseReference = new EdFiCourseReference(courseCode, schoolId, null);
            var schoolReference = new EdFiSchoolReference(schoolId);
            var sessionReference = new EdFiSessionReference(schoolId, srcCourse.SchoolYear.endDate.Year, sessionName);
            return new EdFiCourseOffering(null, courseCode, courseReference, schoolReference, sessionReference,null,null,null, srcCourse.name,
                                            GetEdFiGradeLevelDescriptors(srcCourse.GradeLevels));
        }
        public List<EdFiCourseOfferingOfferedGradeLevel> GetEdFiGradeLevelDescriptors(List<GradeLevel> almaGradeLevels)
        {
            var almaGl = almaGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key).ToList();//Get alma Grade Levels

            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", almaGl);
            var edfiGradeLevels = new List<EdFiCourseOfferingOfferedGradeLevel>();
            foreach (var agl in edfiStringDescriptors)
            {
                edfiGradeLevels.Add(new EdFiCourseOfferingOfferedGradeLevel(agl));
            }
            return edfiGradeLevels;
        }

    }
}
