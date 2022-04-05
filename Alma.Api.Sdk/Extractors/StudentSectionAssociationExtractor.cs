using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Alma.Api.Sdk.Extractors
{
    public interface IStudentSectionAssociationExtractor
    {
        List<StudentSectionResponse> Extract(string almaSchoolCode);
    }

    public class StudentSectionAssociationExtractor : IStudentSectionAssociationExtractor
    {
        private readonly RestClient _client;
        private readonly ISectionsExtractor _sectionsExtractor;
        private readonly ICoursesExtractor _coursesExtractor;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        private readonly IStudentsExtractor _studentsExtractor;        
        private readonly ILogger<StudentSectionAssociationExtractor> _logger;
        public StudentSectionAssociationExtractor(IAlmaRestClientConfigurationProvider client,
                                                  ISectionsExtractor sectionsExtractor,
                                                   ICoursesExtractor coursesExtractor,
                                                   IStudentsExtractor studentsExtractor,
                                                   ILogger<StudentSectionAssociationExtractor> logger,
                                                  ISchoolYearsExtractor schoolYearsExtractor)
        {
            _client = client.GetRestClient();
            _sectionsExtractor = sectionsExtractor;
            _coursesExtractor = coursesExtractor;
            _studentsExtractor = studentsExtractor;
            _schoolYearsExtractor = schoolYearsExtractor;
            _logger = logger;
        }
        public List<StudentSectionResponse> Extract(string almaSchoolCode)
        {
            var almaSections = _sectionsExtractor.Extract(almaSchoolCode);
            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);
            var almaStudents = _studentsExtractor.Extract(almaSchoolCode);
            // Alma courses could be duplicated, so lets reduce the set by just getting the distinct ones.
            var almaCourses = _coursesExtractor.Extract(almaSchoolCode)
                                .GroupBy(x => new { x.schoolYearId, x.id })
                                .Select(g => g.First())
                                .ToList();
            var studentsSection = new ConcurrentBag<StudentSectionResponse>();
            var studentIndex = 0;

            var stopWatch = Stopwatch.StartNew();

            Parallel.ForEach(almaStudents.response, new ParallelOptions { MaxDegreeOfParallelism = 20 },
               student => {
                   var studentSecion = new StudentSectionResponse();
                   studentSecion.StudentId = student.id;
                   studentSecion.classes = GetStudentClasses(almaSchoolCode, student.id, almaSections);
                   if (studentSecion.classes.Count > 0)
                   {
                       studentSecion.classes.ForEach(clas =>
                       {
                           clas.SchoolYear = almaSchoolYears.FirstOrDefault(x => x.id == clas.schoolYearId);
                           if (clas.Course != null)
                               studentsSection.Add(studentSecion);
                           else
                               _logger.LogWarning($"No Courses exist for class {clas.id}- {clas.className}, School Year:{Convert.ToDateTime(clas.SchoolYear.endDate).Year}");

                       });

                   }
                   studentIndex++;
                   if (studentIndex % 10 == 0)
                   {
                       Console.WriteLine($"    Extracting Classes, {studentIndex} students. ({stopWatch.ElapsedMilliseconds} ms   -   {DateTime.Now.ToLongTimeString()})");
                   }
               }
           );
            stopWatch.Stop();
            Console.WriteLine($"    Done in: ({stopWatch.ElapsedMilliseconds / 1000} s.)");
            return studentsSection.ToList();
        }

        private List<Class> GetStudentClasses(string almaSchoolCode, string StudentId, List<Section> almaSections)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/classes", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Class>();
            //Deserialize JSON data
            var classesResponse = new Utf8JsonSerializer().Deserialize<Response<StudentSectionResponse>>(response);
            classesResponse.response.classes.ForEach(clas =>
            {
                if (almaSections.Count(x => x.id == clas.id) > 0)
                {
                    clas.courseId = almaSections.FirstOrDefault(x => x.id == clas.id).courseId;
                    clas.Course = almaSections.FirstOrDefault(x => x.id == clas.id && x.schoolYearId == clas.schoolYearId).Course;
                }
            });

            return classesResponse.response.classes;
        }
    }
}