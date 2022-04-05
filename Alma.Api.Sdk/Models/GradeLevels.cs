using Alma.Api.Sdk.Models;
using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class GradeLevelsResponse
    {
        public List<GradeLevel> response { get; set; }
    }
}

public class GradeLevel
{
    public string id { get; set; }
    public string name { get; set; }
    public string gradeLevelAbbr { get; set; }
    public string equivalent { get; set; }
    public bool nonTeaching { get; set; }
    public string schoolYearId { get; set; }
    public int EdfiSchoolId { get; set; }
    public string AlmaSchoolId { get; set; }
    public SchoolYear SchoolYear { get; set; }
    public string changeLogId { get; set; }
    public string effectiveDate { get; set; }
    public DateTime created { get; set; }
    public DateTime modified { get; set; }
}