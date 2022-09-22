using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class SchoolsResponse
    {
        public string id { get; set; }
        public List<School> schools { get; set; }
    }
    public class School
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string stateId { get; set; }
        public string districtId { get; set; }
        public Settings settings { get; set; }
        public List<Address> addresses { get; set; }
        public List<Phone> phones { get; set; }
        public List<GradeLevel> GradeLevels { get; set; }
        public List<GradingPeriod> gradingPeriods { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
    public class Settings
    {
        public Credits credits { get; set; }
        public Transcripts transcripts { get; set; }
    }
    public class Transcripts
    {
        public bool enabled { get; set; }
        public bool courseAllocation { get; set; }
    }
    public class Credits
    {
        public bool trackCredits { get; set; }
        public float defaultCredits { get; set; }
    }
}
