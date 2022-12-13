using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StudentSectionResponse
    {
        public string id { get; set; }
        public string StudentId { get; set; }
        
        public List<Class> classes { get; set; }
    }

    public class Class
    {
        public string id { get; set; }
        public string schoolYearId { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public string SessionName { get; set; }
        public string className { get; set; }
        public string courseId { get; set; }
        public Course Course { get; set; }
        public List<string> gradingPeriods { get; set; }
        public string status { get; set; }
        public DateTime date { get; set; }
    }
}
