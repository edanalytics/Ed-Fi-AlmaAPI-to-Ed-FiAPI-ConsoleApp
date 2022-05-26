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
    public interface IStudentAttendanceExtractor
    {
        List<Attendance> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StudentAttendanceExtractor : IStudentAttendanceExtractor
    {
        private readonly RestClient _client;
        private readonly ILogger<StudentAttendanceExtractor> _logger;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        private readonly IStudentsExtractor _studentsExtractor;    
        private List<AttendanceForbiden> _attendanceForbiden = new List<AttendanceForbiden>();
        public StudentAttendanceExtractor(IAlmaRestClientConfigurationProvider client,
                                   ISchoolYearsExtractor schoolYearsExtractor,
                                   IStudentsExtractor studentsExtractor,
                                   ILogger<StudentAttendanceExtractor> logger)
        {
            _client = client.GetRestClient();
            _logger = logger;
            _schoolYearsExtractor = schoolYearsExtractor;
            _studentsExtractor = studentsExtractor;
        }
        public List<Attendance> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);

            if (!string.IsNullOrEmpty(schoolYearId))
                almaSchoolYears = almaSchoolYears.Where(x => x.id == schoolYearId).ToList();

            var almaStudents = _studentsExtractor.Extract(almaSchoolCode,schoolYearId);
            var concurrentAttendanceList = new ConcurrentBag<Attendance>();
            var studentIndex = 0;
            var stopWatch = Stopwatch.StartNew();

            Parallel.ForEach(almaStudents.response, new ParallelOptions { MaxDegreeOfParallelism = 2 },
               student => {
                   // Print some feedback
                   studentIndex++;
                   if (studentIndex % 10 == 0)
                       Console.WriteLine($"Extracting Attendance , {studentIndex} students. ({stopWatch.ElapsedMilliseconds} ms - {DateTime.Now.ToLongTimeString()})");

                   foreach (var sYear in almaSchoolYears)
                   {
                       var attendance = GetStudentAttendance(student.id, almaSchoolCode, sYear.id, sYear);
                       if (attendance.Count > 0)
                           attendance.ForEach(a => concurrentAttendanceList.Add(a));
                   }
               }
            );
            if (_attendanceForbiden.Count > 0)
            {
                Console.WriteLine($"  *********** Executing {_attendanceForbiden.Count } Forbiden Attendances.... wait a moment***************");
                var tempAttendances = new ConcurrentBag<Attendance>();
                var attendance = GetForbidenAttendances(tempAttendances);
                if (attendance.Count > 0)
                    attendance.ForEach(a => concurrentAttendanceList.Add(a));
                Console.WriteLine($"  *********** {_attendanceForbiden.Count } Forbiden Attendances were processed correctly***************");
            }
            stopWatch.Stop();
            return concurrentAttendanceList.GroupBy(x => new { x.schoolYearId, x.studentId, x.date, x.status})
                                         .Select(g => g.First()).ToList();
        }

        private List<Attendance> GetStudentAttendance(string StudentId, string almaSchoolCode, string schoolYearId, SchoolYear sYear)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/attendance?schoolYearId={schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                _attendanceForbiden.Add(new AttendanceForbiden { EndPoint= $"v2/{almaSchoolCode}/students/{StudentId}/attendance?schoolYearId={schoolYearId}"
                                                                 ,Success=false,SchoolYear= sYear ,StudentId= StudentId,SchoolCode= almaSchoolCode});
                var error = response.ErrorMessage == null ? response.StatusCode.ToString() : response.ErrorMessage;
                _logger.LogWarning( $" {error} - school:({almaSchoolCode}) year:{sYear.name} (will try again)");
                return new List<Attendance>();
            }                
            //Deserialize JSON data
            var attendanceResponse = new Utf8JsonSerializer().Deserialize<ListResponse<Attendance>>(response);
            attendanceResponse.response.ForEach(c => { c.studentId = StudentId; c.SchoolYear = sYear; });
            
            return attendanceResponse.response;
        }     
        private List<Attendance> GetForbidenAttendances(ConcurrentBag<Attendance> StudentSttendance)
        {
            foreach (var attendance in _attendanceForbiden.Where(x => x.Success == false)) {
                // Print some feedback
                var request = new RestRequest(attendance.EndPoint, DataFormat.Json);
                var response = _client.Get(request);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var error = response.ErrorMessage == null ? response.StatusCode.ToString() : response.ErrorMessage;
                   // _logger.LogWarning($"Fail again ****** {error} ******- school:({attendance.SchoolCode}) year:{attendance.SchoolYear.name}");
                }
                else
                {
                    attendance.Success = true;
                    //Deserialize JSON data
                    var attendanceResponse = new Utf8JsonSerializer().Deserialize<ListResponse<Attendance>>(response);
                    attendanceResponse.response.ForEach(c => { c.studentId = attendance.StudentId; c.SchoolYear = attendance.SchoolYear; StudentSttendance.Add(c); });
                }
            }

           // Parallel.ForEach(_attendanceForbiden.Where(x=>x.Success==false), new ParallelOptions { MaxDegreeOfParallelism = 2 },
           //   attendance => {
           //       // Print some feedback
           //       var request = new RestRequest(attendance.EndPoint, DataFormat.Json);
           //       var response = _client.Get(request);
           //       if (response.StatusCode != HttpStatusCode.OK)
           //       {
           //           var error = response.ErrorMessage == null ? response.StatusCode.ToString() : response.ErrorMessage;
           //           _logger.LogWarning($"Fail again ****** {error} ******- school:({attendance.SchoolCode}) year:{attendance.SchoolYear.name}");
           //       }
           //       else
           //       {
           //           attendance.Success = true;
           //           //Deserialize JSON data
           //           var attendanceResponse = new Utf8JsonSerializer().Deserialize<ListResponse<Attendance>>(response);
           //           attendanceResponse.response.ForEach(c => { c.studentId = attendance.StudentId; c.SchoolYear = attendance.SchoolYear; StudentSttendance.Add(c); });
           //       }
           //   }
           //);
            if (_attendanceForbiden.Count(x => x.Success == false) > 0)
                GetForbidenAttendances(StudentSttendance);

            return StudentSttendance.ToList();
        }
    }
}
