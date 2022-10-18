using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public  class StaffRolResponse
    {
        public StaffRol response { get; set; }
    }
    public class StaffRol
    {
        public List<UserRole> userRoles { get; set; }
    }
}
