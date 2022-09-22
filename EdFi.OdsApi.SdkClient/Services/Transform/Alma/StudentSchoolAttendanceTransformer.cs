using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentSchoolAttendanceTransformer
    {
        EdFiStudentSchoolAttendanceEvent TransformSrcToEdFi(int schoolId, Attendance srcAttendance, List<Session> almaSessions);
    }
    public class StudentSchoolAttendanceTransformer : IStudentSchoolAttendanceTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        private readonly IDescriptorMappingService _descriptorMappingService;
        private readonly ILogger _appLog;
        public StudentSchoolAttendanceTransformer(
            ILoggerFactory logger,
            IDescriptorMappingService descriptorMappingService,
            ISessionNameTransformer sessionNameTransformer)
        {
            _descriptorMappingService = descriptorMappingService;
            _sessionNameTransformer = sessionNameTransformer;
            _appLog = logger.CreateLogger("Student School Attendance Event Transformer");
        }
        public EdFiStudentSchoolAttendanceEvent TransformSrcToEdFi(int schoolId, Attendance srcAttendance, List<Session> almaSessions)
        {
            var sessionName = _sessionNameTransformer.TransformSrcToEdFi(almaSessions, srcAttendance.schoolYearId, srcAttendance.date);
            //Use a helper function to translate the almaID to a StateId.
            StudentTranslation st = new StudentTranslation();
            Student studentResponse = StudentTranslation.GetStudentById(srcAttendance.studentId);
            EdFiStudentReference studentReference = null;
            //Check to see if the returned StateId is null. If it is then try using the AlmaStudentID.
            //This will not insert the student's information but an INFO has already been output for this student during the student posts.
            if (studentResponse.stateId != null)
            {
                studentReference = new EdFiStudentReference(studentResponse.stateId);
            }
            else
            {
                studentReference = new EdFiStudentReference(srcAttendance.studentId);
            }
            //var studentReference = new EdFiStudentReference(srcAttendance.studentId);
            var sessionReference = new EdFiSessionReference(schoolId, srcAttendance.SchoolYear.endDate.Year, sessionName);
            var schoolReference = new EdFiSchoolReference(schoolId);
            return new EdFiStudentSchoolAttendanceEvent(null,
                GetEdFiAttendanceEventCategoryDescriptors( srcAttendance.reportedStatus,srcAttendance.statusModifier),
                srcAttendance.date, schoolReference, sessionReference, studentReference, null, null, null, null);

        }
        public string GetEdFiAttendanceEventCategoryDescriptors(string srcreportedStatus, string srcStatusModifier)
        {
            var attendanceCode = srcreportedStatus;
            if (srcreportedStatus == "Absent")
                if (srcStatusModifier == "Excused" || srcStatusModifier == "Unexcused")
                    srcreportedStatus = $"{srcStatusModifier} {srcreportedStatus}";
            var map = _descriptorMappingService.MappAlmaToEdFiDescriptor("AttendanceEventCategoryDescriptor", srcreportedStatus);
            if (map == null)
                map = _descriptorMappingService.MappAlmaToEdFiDescriptor("AttendanceEventCategoryDescriptor", attendanceCode);
            if (map == null)
                _appLog.LogInformation($"Failed to mapping ---> Reported Status: {attendanceCode} , StatusModifier: {srcStatusModifier}");
            return map;
        }
    }
}
