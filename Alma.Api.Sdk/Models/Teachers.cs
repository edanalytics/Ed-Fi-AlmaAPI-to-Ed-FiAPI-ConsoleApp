using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public  class TeachersResponse
    {
        public List<Teacher> response { get; set; }
    }
}

public class Teacher
{
    public string id { get; set; }
    public string teacherId { get; set; }
    public string startDate { get; set; }
    public string endDate { get; set; }
    public string firstName { get; set; }
    public string middleName { get; set; }
    public string lastName { get; set; }
}