using Alma.Api.Sdk.Extractors.Alma;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Data;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class StudentSectionAttendanceEventProcessor : IProcessor
    {
        public int ExecutionOrder => 130;
        public int RecordIndex = 0;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        public string _almaSchoolCode = "";
        public StudentSectionAttendanceEventProcessor(IAlmaApi almaApi,
                                                        IEdFiApi edFiApi,
                                                        ILoggerFactory logger,
                                                        ILoadExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler;
            _appLog = logger.CreateLogger("Student Section Attendance Event Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Student Section Attendance Event POST new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            _almaSchoolCode = almaSchoolCode;
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students Section Attendance from the source API
            var response = new
            {
                Data = new List<AlmaStudentSectionAttendanceEvent> {
                    new AlmaStudentSectionAttendanceEvent {
                        LocalCourseCode= AlmaFakeData.LocalCourseCode, SectionIdentifier= AlmaFakeData.SectionIdentifier, SchoolId = AlmaFakeData.SchoolId, Descriptor = AlmaFakeData.AttendanceEventCategoryDescriptor, StudentId = AlmaFakeData.StudentId1, SchoolYear = AlmaFakeData.SchoolYear, EventDate =AlmaFakeData.EventDate, SessionName =AlmaFakeData.SessionName } },
                Headers = new Dictionary<string, string> { { "Total-Count", "1" } }
            };
            // Transform
            var mapped = response.Data.Select(x => Map(x)).ToList();
            // Load
            mapped.ForEach(x => Load(x));
            var totalRowsAvaialable = Convert.ToInt32(response.Headers["Total-Count"]);
        }
        private EdFiStudentSectionAttendanceEvent Map(AlmaStudentSectionAttendanceEvent resource)
        {
            var sectionReference = new EdFiSectionReference(resource.LocalCourseCode, resource.SchoolId, resource.SchoolYear, resource.SectionIdentifier, resource.SessionName, null);
            var studentReference = new EdFiStudentReference(resource.StudentId);
            return new EdFiStudentSectionAttendanceEvent(null, EdFiAttendanceEventCategoryDescriptorTranslator(resource.Descriptor), resource.EventDate, sectionReference, studentReference);
        }

        private void Load(EdFiStudentSectionAttendanceEvent resource)
        {
            try
            {
               
                //var result = _apiEdFi.SSectionAttendance.PostStudentSectionAttendanceEventWithHttpInfo(resource);
                //_exceptionHandler.HandleHttpCode(result);
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogWarning( $"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
            }
        }

        private string EdFiAttendanceEventCategoryDescriptorTranslator(string descriptor)
        {
            var result = String.Empty;
            switch (descriptor)
            {
                case "absense":
                    result= descriptor = "uri://ed-fi.org/AttendanceEventCategoryDescriptor#Unexcused Absence";
                    break;
                default:
                    throw new NotImplementedException($"{descriptor} has no ed-fi translation.");
            }

            return result;
        }
        public class AlmaStudentSectionAttendanceEvent
        {
            public int SchoolId { get; set; }
            public string StudentId { get; set; }
            public DateTime? EventDate { get; internal set; }
            public int? SchoolYear { get; internal set; }
            public string SessionName { get; internal set; }
            public string Descriptor { get; internal set; }
            public string LocalCourseCode { get; internal set; }
            public string SectionIdentifier { get; internal set; }
        }
    }
}
