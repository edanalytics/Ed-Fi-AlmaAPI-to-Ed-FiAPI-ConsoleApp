using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ICourseOfferingTransformer
    {
        //EdFiCourseOffering TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions);
        List<EdFiCourseOffering> TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions);
        List<EdFiCourseOffering> TransformSrcToEdFi(int schoolId, Section srcSection, List<Session> almaSessions);
    }
    public class CourseOfferingTransformer : ICourseOfferingTransformer
    {
        private readonly ISessionNameTransformer _sessionTransformer;
        private readonly IDescriptorMappingService _descriptorMappingService;


        public CourseOfferingTransformer(
            ISessionNameTransformer sessionNameTransformer,
            IDescriptorMappingService descriptorMappingService)
        {
            _sessionTransformer = sessionNameTransformer;
            _descriptorMappingService = descriptorMappingService;
        }


        public EdFiCourseOffering TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions)
        {
            var sessionName = _sessionTransformer.TransformSrcToEdFi(almaSessions, srcCourse.schoolYearId, srcCourse.effectiveDate);
            var courseCode = string.IsNullOrEmpty(srcCourse.code) ? srcCourse.id : srcCourse.code;
            var courseReference = new EdFiCourseReference(courseCode, schoolId, null);
            var schoolReference = new EdFiSchoolReference(schoolId);
            var sessionReference = new EdFiSessionReference(schoolId, srcCourse.SchoolYear.endDate.Year, sessionName);
            return new EdFiCourseOffering(null, courseCode, courseReference, schoolReference, sessionReference, null, null, null, srcCourse.name,
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

        List<EdFiCourseOffering> ICourseOfferingTransformer.TransformSrcToEdFi(int schoolId, Course srcCourse, List<Session> almaSessions)
        {
            var EedFiCoursesOffering = new List<EdFiCourseOffering>();
            var courseCode = string.IsNullOrEmpty(srcCourse.code) ? srcCourse.id : srcCourse.code;
            var courseReference = new EdFiCourseReference(courseCode, schoolId, null);
            var schoolReference = new EdFiSchoolReference(schoolId);
            foreach (var term in srcCourse.gradingPeriods)
            {
                EedFiCoursesOffering.Add(new EdFiCourseOffering(null, courseCode, courseReference, schoolReference,
                                         new EdFiSessionReference(schoolId, srcCourse.SchoolYear.endDate.Year, _sessionTransformer.TransformSrcToEdFi(term, almaSessions)), null, null, null, srcCourse.name,
                                            GetEdFiGradeLevelDescriptors(srcCourse.GradeLevels)));

            }

            return EedFiCoursesOffering;
        }

        public List<EdFiCourseOffering> TransformSrcToEdFi(int schoolId, Section srcSection, List<Session> almaSessions)
        {
            var EedFiCoursesOffering = new List<EdFiCourseOffering>();
            var courseCode = string.IsNullOrEmpty(srcSection.Course.code) ? srcSection.Course.id : srcSection.Course.code;
            var courseReference = new EdFiCourseReference(courseCode, schoolId, null);
            var schoolReference = new EdFiSchoolReference(schoolId);
            if (srcSection.gradingPeriods != null) //TODO: Figure out why these are null.
            {
                foreach (var term in srcSection.gradingPeriods)
                {
                    EedFiCoursesOffering.Add(new EdFiCourseOffering(null, courseCode, courseReference, schoolReference,
                                             new EdFiSessionReference(schoolId, srcSection.Course.SchoolYear.endDate.Year, _sessionTransformer.TransformSrcToEdFi(term, almaSessions)), null, null, null, srcSection.Course.name,
                                                GetEdFiGradeLevelDescriptors(srcSection.Course.GradeLevels)));

                }
            }

            return EedFiCoursesOffering;
        }
    }
}
