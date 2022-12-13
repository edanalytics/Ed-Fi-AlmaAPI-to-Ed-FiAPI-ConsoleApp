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
    public interface ISectionsExtractor
    {

        List<Section> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class SectionsExtractor : ISectionsExtractor
    {
        private readonly RestClient _client;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        private readonly ICoursesExtractor _coursesExtractor;
        private readonly ILogger<SectionsExtractor> _logger;

        public SectionsExtractor(IAlmaRestClientConfigurationProvider client,
                                 ISchoolYearsExtractor schoolYearsExtractor,
                                 ILogger<SectionsExtractor> logger,
                                 ICoursesExtractor coursesExtractor)
        {
            _client = client.GetRestClient();
            _schoolYearsExtractor = schoolYearsExtractor;
            _coursesExtractor = coursesExtractor;
            _logger = logger;
        }

        public List<Section> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);
            var almaCourses = _coursesExtractor.Extract(almaSchoolCode)
                                .GroupBy(x => new { x.schoolYearId, x.id })
                                .Select(g => g.First())
                                .ToList();
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{almaSchoolCode}/classes{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            var classesResponse = new Utf8JsonSerializer().Deserialize<SectionsResponse>(response);

            var classesList = new List<Section>();
            classesResponse.response.ForEach(c =>
            {
                // Add the schoolyear
                c.SchoolYear = almaSchoolYears.FirstOrDefault(sy => sy.id == c.schoolYearId);
                var almaCourse = almaCourses.FirstOrDefault(cour => cour.id == c.courseId && cour.schoolYearId == c.schoolYearId);
                if (almaCourse == null)
                    _logger.LogWarning($"{almaSchoolCode}/courses/{c.courseId}:  No Courses exist for courseId:{c.courseId} ,class {c.id}- {c.name}, School Year:{Convert.ToDateTime(c.SchoolYear.endDate).Year}");
                else
                {
                    c.Course = almaCourse;
                    classesList.Add(c);
                }
            });
            return classesList;
        }

        private List<Teacher> GetTeachers(string almaSchoolCode, string SectionId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/classes/{SectionId}/teachers", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var CourseResponse = new Utf8JsonSerializer().Deserialize<TeachersResponse>(response);
            return CourseResponse.response;
        }
    }
}
