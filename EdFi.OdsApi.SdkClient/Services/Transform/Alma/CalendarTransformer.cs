using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ICalendarTransformer
    {
        List<EdFiCalendar> TransformSrcToEdFi(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents);
    }
    public class CalendarTransformer : ICalendarTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public CalendarTransformer(
        IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public List<EdFiCalendar> TransformSrcToEdFi(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents)
        {
            var edFiCalendars = almaCalendarEvents
                    .GroupBy(x => new { x.schoolYearId })
                    .Select(g =>
                            new EdFiCalendar(null, $"{almaSchoolCode}-{g.FirstOrDefault().SchoolYear.endDate.Year}",
                                        new EdFiSchoolReference(schoolId),
                                        new EdFiSchoolYearTypeReference(g.FirstOrDefault().SchoolYear.endDate.Year),
                                        GetEdFiCalendarTypeDescriptors("school")));

            return edFiCalendars.ToList();
        }

        public string GetEdFiCalendarTypeDescriptors(string srcCalendarType)
        {
           return _descriptorMappingService.MappAlmaToEdFiDescriptor("CalendarTypeDescriptor", srcCalendarType);
        }
    }
}
