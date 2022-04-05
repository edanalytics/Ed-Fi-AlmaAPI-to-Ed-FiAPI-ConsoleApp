using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class StaffSection
    {
        public string id { get; set; }
        public string StaffId { get; set; }
        public string Rol { get; set; }
        public string roleId { get; set; }
        public List<StaffClass> classes { get; set; }
    }

}
