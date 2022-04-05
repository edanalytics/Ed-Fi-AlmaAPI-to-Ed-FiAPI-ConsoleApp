using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class EmailsResponse
    {
        public List<Email> email { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
    public class Email
    {
        public string emailAddress { get; set; }
    }
}
