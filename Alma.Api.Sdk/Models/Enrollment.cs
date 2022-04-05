using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class EnrollmentResponse
    { 
        public List<Enrollment> response { get; set; }
    }


    public class Enrollment
    { 
        public string id { get; set; }
        public string codeId { get; set; }
        public string comment { get; set; }
        public DateTime created { get; set; }
        public string creator { get; set; }
        public string date { get; set; }
        public string school { get; set; }
        public string studentId { get; set; }
        public string type { get; set; }
    }
}
