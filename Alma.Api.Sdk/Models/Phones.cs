using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class PhonesResponse
    {
        public List<Phone> phones  { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
    public class Phone
    {
        public string number { get; set; }
        public bool canSms { get; set; }
        public string type { get; set; }
    }
}
