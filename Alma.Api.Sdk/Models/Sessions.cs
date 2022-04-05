using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class SessionsResponse
    {
        public List<Session> response { get; set; }

    }
    public class Session
    {
        public string id { get; set; }
        public string type { get; set; }
        public string schoolYearId { get; set; }
        public List<Term> terms { get; set; }
        public SchoolYear SchoolYear { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

    public class Term
    {
        public string termId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public DateTime startDate { get; set; }
        public DateTime endDate { get; set; }
        public int totalInstructionalDays { get; set; }
        
    }

    public class SchoolYear
    {
        public string id { get; set; }
        public string name { get; set; }
        public DateTime startDate { get; set; }
        public int EdfiSchoolId { get; set; }
        public string AlmaSchoolId { get; set; }
        public DateTime endDate { get; set; }
        public string status { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

}
