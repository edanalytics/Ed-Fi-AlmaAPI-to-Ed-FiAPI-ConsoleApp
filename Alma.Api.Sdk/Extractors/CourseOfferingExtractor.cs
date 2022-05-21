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

        public CourseOfferingExtractor(IAlmaRestClientConfigurationProvider client,
                                       ICoursesExtractor coursesExtractor,
                                       ILogger<CourseOfferingExtractor> logger)
        {
            _client = client.GetRestClient();
            _coursesExtractor = coursesExtractor;
            _logger = logger;
        }
        public List<Course> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            // Alma courses could be duplicated, so lets reduce the set by just getting the distinct ones.
            var almaCourses = _coursesExtractor.Extract(almaSchoolCode, schoolYearId)
                                .GroupBy(x => new { x.schoolYearId, x.id})                               
                                .Select(g=>g.First())
                                .ToList();
            //Exists any filter for School Year????
            if (!string.IsNullOrEmpty(schoolYearId))
            {
                schoolYearId = $"?schoolYearId={schoolYearId}";
            }
            // We are getting the Classes for alma and deriving the courses from there.
            // This way we know for a fact that the course is being offered.
            var request = new RestRequest($"v2/{almaSchoolCode}/classes{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            var classesResponse = new Utf8JsonSerializer().Deserialize<SectionsResponse>(response);
            
            Console.WriteLine($"Processing Courses from {classesResponse.response.Count} Classes.");

            var courseList = new List<Course>();
            classesResponse.response.ForEach(c =>
            {
                var courses = almaCourses.Where(ac => ac.id == c.courseId && ac.schoolYearId==c.schoolYearId);

                if (courses.Count() <= 0)
                    _logger.LogWarning($"{almaSchoolCode}/courses/{c.courseId}  :  No Courses exist for courseId:{c.courseId} into the class {c.id}- {c.name}, School Year:{Convert.ToDateTime(c.SchoolYear).Year}");
                else
                    courseList.AddRange(courses);
            });
            return courseList;
        }
    }
}

