using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{

    public class StudentsResponse
    {
        public List<Student> response { get; set; }
        public List<Link> _links { get; set; }
    }
    public class Student
    {
        public string id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string districtId { get; set; }
        public string stateId { get; set; }
        public string schoolId { get; set; }
        public string gender { get; set; }
        public string ethnicity { get; set; }        
        public List<string> race { get; set; }
        public string dob { get; set; }
        public string status { get; set; }
        public List<Address> addresses { get; set; }
        public List<Phone> phones { get; set; }
        public List<Email> emails { get; set; }
        public List<Enrollment> Enrollment { get; set; }
        public List<GradeLevel> GradeLevels { get; set; }
        public ProgramsResponse Programs { get; set; }
        public List<object> externalIds { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }

 

}
