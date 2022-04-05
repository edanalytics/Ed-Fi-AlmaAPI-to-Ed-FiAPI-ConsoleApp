using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public  class GradingPeriodsResponse
    {
        public GradingPeriod response { get; set; }
        public List<Link> _links { get; set; }
    }
    public class GradingPeriod
    {
        public string id { get; set; }
        public string type { get; set; }
        public string EdfiSchoolId   { get; set; }
        public string schoolYearId { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public List<Term> terms { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

}

