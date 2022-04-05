using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class AttendanceResponse
    {
        public List<Attendance> response { get; set; }
        public List<SchoolYear>   SchoolYears { get; set; }
    }
    public class Attendance
    {
        public string id { get; set; }
        public string schoolYearId { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public string schoolId { get; set; }
        public string studentId { get; set; }
        public DateTime date { get; set; }
        public string attendanceCodeId { get; set; }
        public bool adminLocked { get; set; }
        public int minutes { get; set; }
        public int minutesExpected { get; set; }
        public string note { get; set; }
        public string reportedStatus { get; set; }
        public string status { get; set; }
        public string statusModifier { get; set; }        
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

}
