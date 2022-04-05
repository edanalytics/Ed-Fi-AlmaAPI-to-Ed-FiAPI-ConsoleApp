using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class AddressessResponse
    {
        public AddressessResponse()
        {
            addresses = new List<Address>();
        }
        public List<Address> addresses { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
    public class Address
    {
        public string address { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public List<string> type { get; set; }
    }
}
