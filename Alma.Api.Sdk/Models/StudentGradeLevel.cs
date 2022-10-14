using Alma.Api.Sdk.Models;
using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StudentGradeLevelsResponse
    {
        public List<StudentG> students { get; set; }
    }
}

public class StudentGradeLevel
{
    public string id { get; set; }
    public string gradeLevelId { get; set; }
    public string comment { get; set; }
    public string creator { get; set; }
    public string schoolYearId { get; set; }
    public string school { get; set; }
    public string effectiveDate { get; set; }
    public DateTime created { get; set; }
}

public class StudentG
{
    public string id { get; set; }
    public List<StudentGradeLevel> gradeLevels { get; set; }
}
