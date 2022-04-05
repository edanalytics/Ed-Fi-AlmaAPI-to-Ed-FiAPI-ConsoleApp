using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class UserRoleResponse
    {
        public List<UserRole> userRoles { get; set; }
    }

    public class UserRole
    {
        public string id { get; set; }
        public string name { get; set; }
        public string userType { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}
