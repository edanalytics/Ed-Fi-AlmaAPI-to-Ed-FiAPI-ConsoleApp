using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class CoursesResponse
    {
        public List<Course> response { get; set; }
    }

    public class Course
    {
        public string id { get; set; }
        public string almaClassId { get; set; }
        public DateTime effectiveDate { get; set; }
        public string name { get; set; }
        public decimal length { get; set; }
        public string gpaScaleId { get; set; }
        public decimal creditHours { get; set; }
        public bool showOnTranscript { get; set; }
        public string creditAllocation { get; set; }
        public string subjectId { get; set; }
        public string schoolYearId { get; set; }
        public string AlmaSchoolId { get; set; }

        public string code { get; set; }
        public string description { get; set; }
        public List<string> gradeLevelIds { get; set; }
        public List<string> gradingPeriods { get; set; }

        public SchoolYear SchoolYear { get; set; }
        public List<Subject> Subjects { get; set; }
        public List<GradeLevel> GradeLevels { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}
