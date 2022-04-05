using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ICalendarEventTransformer
    {
        List<EdFiCalendarDateCalendarEvent> TransformSrcToEdFi(List<CalendarEvent> almaCalendarEvents);
    }
    public class CalendarEventTransformer : ICalendarEventTransformer
    {
        private readonly IDescriptorMapping _calendarEvent;
        public CalendarEventTransformer(IOptions<CalendarEventMapping> mapping)
        {
            _calendarEvent = mapping.Value;
        }
        public List<EdFiCalendarDateCalendarEvent> TransformSrcToEdFi(List<CalendarEvent> almaCalendarEvents)
        {
            return almaCalendarEvents.Select(ce => MapAlmaCalendarEventToEdFiCalendarEvent(ce.EventType.name)).ToList();
        }

        private EdFiCalendarDateCalendarEvent MapAlmaCalendarEventToEdFiCalendarEvent(string almaEventName)
        {
            var map = _calendarEvent.Mapping.SingleOrDefault(x => x.Src == almaEventName.ToLower());
            if (map == null)
                map = _calendarEvent.Mapping.SingleOrDefault(x => x.Src == "default");
           return  new EdFiCalendarDateCalendarEvent(map.Dest);
        }
    }
}
