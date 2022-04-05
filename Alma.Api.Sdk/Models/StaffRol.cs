using System;

namespace Alma.Api.Sdk.Models
{
    public  class StaffRolResponse
    {
        public StaffRol response { get; set; }
    }
    public class StaffRol
    {
        public string id { get; set; }
        public string name { get; set; }
        public string userType { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}
