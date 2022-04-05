using Alma.Api.Sdk.Models;
using System;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class CalendarEventsResponse
    {
        public List<CalendarEvent> response { get; set; }
    }
}
 
public class CalendarEvent
{
    public string id { get; set; }
    public string eventTypeId { get; set; }
    public string schoolYearId { get; set; }
    public string name { get; set; }
    public DateTime startDate { get; set; }
    public DateTime endDate { get; set; }
    public SchoolYear SchoolYear { get; set; }
    public EventTypes EventType { get; set; }
    public DateTime created { get; set; }
    public DateTime modified { get; set; }
} 