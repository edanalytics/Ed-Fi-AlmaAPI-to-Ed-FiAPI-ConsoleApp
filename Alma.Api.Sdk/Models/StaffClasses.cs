using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StaffClassesResponse
    {
        public string id { get; set; }
        public List<StaffClass> classes { get; set; }
    }

    public class StaffClass
    {
        public string teacherEnrollmentId { get; set; }
        public string schoolYearId { get; set; }
        public string id { get; set; }
        public Course Course{ get; set; }
        public List<string> gradingPeriods { get; set; }
        public string ClassName { get; set; }        
        public string  startDate { get; set; }
        public string  endDate { get; set; }
        public bool isPrimary { get; set; }
    }

}
