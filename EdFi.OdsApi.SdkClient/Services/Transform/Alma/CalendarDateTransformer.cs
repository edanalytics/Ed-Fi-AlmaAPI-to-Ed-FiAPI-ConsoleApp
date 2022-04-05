using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ICalendarDateTransformer
    {
        List<EdFiCalendarDate> TransformSrcToEdFi(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents);
    }
    public class CalendarDateTransformer : ICalendarDateTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public CalendarDateTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public List<EdFiCalendarDate> TransformSrcToEdFi(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents)
        {
            var calendarDates = almaCalendarEvents
                        .GroupBy(x => new { x.schoolYearId, x.startDate, x.endDate, x.EventType })
                        .Select(g => new EdFiCalendarDate(null,
                                      GetEdFiCalendarEventDescriptors(g.ToList()),
                                     g.Key.startDate,
                                     new EdFiCalendarReference($"{almaSchoolCode}-{g.FirstOrDefault().SchoolYear.endDate.Year}", schoolId, g.FirstOrDefault().SchoolYear.endDate.Year)
                                     )).ToList();
            return calendarDates;
        }

        public List<EdFiCalendarDateCalendarEvent> GetEdFiCalendarEventDescriptors(List<CalendarEvent> almaCalendarEvents)
        {
            return almaCalendarEvents.Select(ce => MapAlmaCalendarEventToEdFiCalendarEvent(ce.EventType.name)).ToList();
        }

        private EdFiCalendarDateCalendarEvent MapAlmaCalendarEventToEdFiCalendarEvent(string almaEventName)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("CalendarEventDescriptor", almaEventName.ToLower());
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("CalendarEventDescriptor", "default");
            return new EdFiCalendarDateCalendarEvent(edfiStringDescriptors);
        }
    }
}
