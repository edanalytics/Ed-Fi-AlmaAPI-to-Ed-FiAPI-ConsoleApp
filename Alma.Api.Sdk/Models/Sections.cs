using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class SectionsResponse
    {
        public List<Section> response { get; set; }
    }
    public class Section
    {
        public string id { get; set; }
        public string name { get; set; }
        public string courseId { get; set; }
        public string gradingScaleId { get; set; }
        public string AlmaSchoolId { get; set; }
        public List<string> gradingPeriods { get; set; }
        public GradingPeriodsResponse gradingPer { get; set; }
        public string schoolYearId { get; set; }
        public string school { get; set; }
        public string type { get; set; }
        public Settings settings { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public List<Teacher> Teachers { get; set; }
        public Course Course { get; set; }
        public List<object> externalIds { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
        public List<Link> _links { get; set; }
    }
}