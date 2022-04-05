using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StudentSchoolAssociationResponse
    {
        public string studentId { get; set; }
        public List<Enrollment> enrolments { get; set; }
    }
}
