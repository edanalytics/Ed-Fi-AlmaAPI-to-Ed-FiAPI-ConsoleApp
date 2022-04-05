using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StaffResponse
    {
        public List<Staff> response { get; set; }
    }
    public class Staff
    {
        public string id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string middleName { get; set; }
        public string lastName { get; set; }
        public string schoolId { get; set; }
        public string districtId { get; set; }
        public string stateId { get; set; }
        public string gender { get; set; }
        public string dob { get; set; }
        public string roleId { get; set; }
        public string Rol { get; set; }
        public string status { get; set; }
        public List<object> externalIds { get; set; }
        public List<Address> addresses { get; set; }
        public List<Phone> phones { get; set; }
        public List<Email> emails { get; set; }
        public List<StaffClass> classes { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}
