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
    public interface IStaffSectionAssociationExtractor
    {
        List<StaffSection> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StaffSectionAssociationExtractor : IStaffSectionAssociationExtractor
    {
        private readonly RestClient _client;
        private readonly ISectionsExtractor _sectionsExtractor;
        private readonly ICoursesExtractor _coursesExtractor;
        private readonly ILogger<SectionsExtractor> _logger;
        private readonly IStaffsExtractor _staffsExtractor;
        public StaffSectionAssociationExtractor(IAlmaRestClientConfigurationProvider client,
                                                ISectionsExtractor sectionsExtractor,
                                                ICoursesExtractor coursesExtractor,
                                                IStaffsExtractor staffsExtractor,
                                                ILogger<SectionsExtractor> logger)
        {
            _client = client.GetRestClient();
            _sectionsExtractor = sectionsExtractor;
            _coursesExtractor = coursesExtractor;
            _staffsExtractor = staffsExtractor;
            _logger = logger;
        }
        public List<StaffSection> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var schoolYearIdFilter = "";
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearIdFilter = $"?schoolYearId={schoolYearId}";

            var almaSections = _sectionsExtractor.Extract(almaSchoolCode, schoolYearId);
            //var almaCourses = _coursesExtractor.Extract(almaSchoolCode)
            //                  .GroupBy(x => new { x.schoolYearId, x.id })
            //                  .Select(g => g.First())
            //                  .ToList();
            var staffs = _staffsExtractor.Extract(almaSchoolCode, schoolYearId);
            var staffsSection = new ConcurrentBag<StaffSection>();
            var studentIndex = 0;
            var stopWatch = Stopwatch.StartNew();
            Parallel.ForEach(staffs.response, new ParallelOptions { MaxDegreeOfParallelism = 10 },
               staff => {
                   studentIndex++;
                   var staffSecion = new StaffSection();
                   staffSecion.StaffId = staff.id;
                   staffSecion.roleId = staff.roleId;
                   staffSecion.classes = GetStaffClasses(almaSchoolCode, staff.id, almaSections, schoolYearId);
                   if (staffSecion.classes.Count > 0)
                   {
                       staffSecion.classes.ForEach(clas =>
                       {
                           if (clas.Course != null)
                               staffsSection.Add(staffSecion);
                           else
                               _logger.LogWarning($"{almaSchoolCode}/staff/{staff.id}/classes{schoolYearIdFilter} :  No Courses exist for ClassId {clas.id} or the ClassId is incorrect, School {almaSchoolCode}");
                       });
                   }
                   if (studentIndex % 10 == 0)
                   {
                       Console.WriteLine($"    Extracting Staff Classes {studentIndex} of {staffs.response.Count} Staffs. ({stopWatch.ElapsedMilliseconds} ms - {DateTime.Now.ToLongTimeString()})");
                   }
               }
           );
            stopWatch.Stop();
            Console.WriteLine($"    Done in: ({stopWatch.ElapsedMilliseconds / 1000} s.)");
            return staffsSection.ToList();
        }

        private List<StaffClass> GetStaffClasses(string almaSchoolCode, string StaffId, List<Section> almaSections, string schoolYearId)
        {
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{almaSchoolCode}/staff/{StaffId}/classes{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new List<StaffClass>();

            //Deserialize JSON data
            var classesResponse = new Utf8JsonSerializer().Deserialize<Response<StaffClassesResponse>>(response);
            classesResponse.response.classes.ForEach(clas =>
            {
                if (almaSections.Count(x => x.id == clas.id && x.schoolYearId == clas.schoolYearId) > 0)
                {
                    clas.Course = almaSections.FirstOrDefault(x => x.id == clas.id && x.schoolYearId == clas.schoolYearId).Course;
                    clas.ClassName = almaSections.FirstOrDefault(sec => sec.id == clas.id).name;
                    clas.gradingPeriods = almaSections.FirstOrDefault(x => x.id == clas.id && x.schoolYearId == clas.schoolYearId).gradingPeriods;
                }
            });
            return classesResponse.response.classes;
        }
    }
}
