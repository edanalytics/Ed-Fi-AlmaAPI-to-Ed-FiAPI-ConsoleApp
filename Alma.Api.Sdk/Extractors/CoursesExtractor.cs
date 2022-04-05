using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Api.Sdk.Extractors
{
    public interface ICoursesExtractor
    {
        
        List<Course> Extract(string almaSchoolCode);
    }

    public class CoursesExtractor : ICoursesExtractor
    {
        private readonly RestClient _client;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        private readonly IGradeLevelsExtractor _gradeLevelsExtractor;
        private readonly ISubjectsExtractor _subjectsExtractor;
        public CoursesExtractor(IAlmaRestClientConfigurationProvider client,
                                ISchoolYearsExtractor schoolYearsExtractor,
                                ISessionsExtractor sessionsExtractor,
                                IGradeLevelsExtractor gradeLevelsExtractor,
                                ISubjectsExtractor subjectsExtractor)
        {
            _client = client.GetRestClient();
            _schoolYearsExtractor = schoolYearsExtractor;
            _gradeLevelsExtractor = gradeLevelsExtractor;
            _subjectsExtractor = subjectsExtractor;
        }
        public List<Course> Extract(string almaSchoolCode)
        {
            // Call other extractors we are going to need.
            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);
            // Note: That for some reason in Alma there are multiple grade levels with the same Id. 
            // Seems to be because of the valid since date but the rest looks identical so we are grouping by the unique Id.
            var almaGradeLevels = _gradeLevelsExtractor.Extract(almaSchoolCode).GroupBy(x => x.id).Select(g => g.First()).ToList();
            var almaSubjectss = _subjectsExtractor.Extract(almaSchoolCode);
            
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{almaSchoolCode}/courses", DataFormat.Json);
            //Synchronous call
            var response = _client.Get(request);
            //Deserialize JSON data
            var courseResponse = new Utf8JsonSerializer().Deserialize<CoursesResponse>(response);

            Console.WriteLine($"Processing {courseResponse.response.Count} Courses.");
            
            courseResponse.response.ForEach(c =>
            {
                c.SchoolYear = almaSchoolYears.FirstOrDefault(sy => sy.id == c.schoolYearId);
                c.Subjects = almaSubjectss.Where(s => s.id == c.subjectId).ToList();
                c.GradeLevels = almaGradeLevels.Where(gL => c.gradeLevelIds.Contains(gL.id)).ToList();
            });

            return courseResponse.response;
        }
    }
}

