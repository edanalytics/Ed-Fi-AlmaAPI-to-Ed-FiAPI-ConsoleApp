using System;

namespace Alma.Api.Sdk.Models
{
    public class EventTypes
    {
        public string id { get; set; }
        public string name { get; set; }
        public bool blocking { get; set; }
        public DateTime created { get; set; }
        public DateTime modified { get; set; }
    }
}
