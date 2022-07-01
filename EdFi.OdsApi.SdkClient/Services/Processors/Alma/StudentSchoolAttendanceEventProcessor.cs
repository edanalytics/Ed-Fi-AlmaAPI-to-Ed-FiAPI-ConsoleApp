using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class StudentSchoolAttendanceEventProcessor : IProcessor
    {
        public int ExecutionOrder => 110;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        
        
        
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentSchoolAttendanceTransformer _schoolAttendanceTransformer;
        public StudentSchoolAttendanceEventProcessor(IAlmaApi almaApi, 
                                                     IEdFiApi edFiApi,
                                                     IStudentSchoolAttendanceTransformer schoolAttendanceTransformer,
                                                     ILoggerFactory logger,
                                                     ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _schoolAttendanceTransformer = schoolAttendanceTransformer;
            _appLog = logger.CreateLogger("Student School Attendance Event Processor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Student School Attendance Event from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // Extract - Get students School from the source API
            //var staffResponse = _apiAlma.Staff.Extract(almaSchoolCode,schoolYearId);
            //get sessions to relate attendance to a session
            var almaStudentAttendanceResponse = _apiAlma.Attendance.Extract(almaSchoolCode,schoolYearId);
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            var AttendancetoRequest = Transform(stateSchoolId, almaStudentAttendanceResponse, almaSessions);
            AttendancetoRequest.ForEach(x => Load(x, almaSchoolCode));

            //Parallel.ForEach(AttendancetoRequest,
            //  new ParallelOptions { MaxDegreeOfParallelism = 3 }, attendance => {
            //      Load(attendance, almaSchoolCode);
            //  });
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStudentAttendanceResponse.Count} School Attendance rows .");
        }

        private List<EdFiStudentSchoolAttendanceEvent> Transform(int stateSchoolId,List<Attendance> studentAttendance, List<Session> almaSections)
        {
            var edfiAttendanceDate = new List<EdFiStudentSchoolAttendanceEvent>();
            studentAttendance.ForEach(x => edfiAttendanceDate.Add(_schoolAttendanceTransformer.TransformSrcToEdFi(stateSchoolId,x, almaSections)));
            return edfiAttendanceDate;
        }
        private void Load(EdFiStudentSchoolAttendanceEvent resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.TokenNeedsToRenew())
                    _apiEdFi.RenewToken();

                var result = _apiEdFi.SSchoolAttendance.PostStudentSchoolAttendanceEventWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    var attenDanceEvent = resource.AttendanceEventCategoryDescriptor.Split("#");
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Students School Attendance registered(Last event registered for Student Id:{resource.StudentReference.StudentUniqueId}, {resource.EventDate.Value.ToShortDateString() } - {attenDanceEvent[1]})");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError( $"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
            }
        }
    }
}
