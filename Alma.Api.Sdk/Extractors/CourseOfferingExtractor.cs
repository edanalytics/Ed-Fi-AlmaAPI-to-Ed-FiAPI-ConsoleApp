using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Alma.Api.Sdk.Extractors
{
    public interface ICourseOfferingExtractor
    {
        List<Course> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class CourseOfferingExtractor : ICourseOfferingExtractor
    {
        private readonly RestClient _client;
        private readonly ICoursesExtractor _coursesExtractor;
        private readonly ILogger<CourseOfferingExtractor> _logger;
        private readonly ISectionsExtractor _sectionsExtractor;

        public CourseOfferingExtractor(IAlmaRestClientConfigurationProvider client,
                                       ICoursesExtractor coursesExtractor,
                                       ISectionsExtractor sectionsExtractor,
                                       ILogger<CourseOfferingExtractor> logger)
        {
            _client = client.GetRestClient();
            _coursesExtractor = coursesExtractor;
            _logger = logger;
            _sectionsExtractor = sectionsExtractor;
        }
        public List<Course> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            // Alma courses could be duplicated, so lets reduce the set by just getting the distinct ones.
            var almaCourses = _coursesExtractor.Extract(almaSchoolCode, schoolYearId)
                                .GroupBy(x => new { x.schoolYearId, x.id })
                                .Select(g => g.First())
                                .ToList();
            var almaSections = _sectionsExtractor.Extract(almaSchoolCode, schoolYearId);
            var courseList = new List<Course>();
            almaSections.ForEach(classItem =>
            {
                var courseItem = new Course();
                courseItem.almaClassId = classItem.id;
                courseItem = classItem.Course;
                //We need to know the grading periods to get the session name
                courseItem.gradingPeriods = classItem.gradingPeriods;
                courseList.Add(courseItem);
            });
            return courseList;
        }
    }
}

