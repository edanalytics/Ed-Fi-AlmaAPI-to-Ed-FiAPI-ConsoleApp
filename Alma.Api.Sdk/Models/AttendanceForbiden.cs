using System;
using System.Collections.Generic;
using System.Text;

namespace Alma.Api.Sdk.Models
{
   public class AttendanceForbiden
    {
        public string SchoolCode { get; set; }
        public string EndPoint { get; set; }
        public string StudentId { get; set; }
        public bool Success { get; set; }
        public SchoolYear SchoolYear { get; set; }

    }
}
