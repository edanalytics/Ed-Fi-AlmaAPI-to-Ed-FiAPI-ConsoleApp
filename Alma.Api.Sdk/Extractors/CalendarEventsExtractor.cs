using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Linq;

namespace Alma.Api.Sdk.Extractors
{
    public interface ICalendarEventsExtractor
    {
        
        CalendarEventsResponse Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class CalendarEventsExtractor : ICalendarEventsExtractor
    {
        private readonly RestClient _client;
        private readonly ICalendarEventTypesExtractor _calendarEventTypesExtractor;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;

        public CalendarEventsExtractor(IAlmaRestClientConfigurationProvider client,
                                  ISchoolYearsExtractor schoolYearsExtractor,
                                  ICalendarEventTypesExtractor calendarEventTypesExtractor)
        {
            _client = client.GetRestClient();
            _calendarEventTypesExtractor = calendarEventTypesExtractor;
            _schoolYearsExtractor = schoolYearsExtractor;
        }
        public CalendarEventsResponse Extract(string almaSchoolCode, string schoolYearId = "")
        {
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";

            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);
            var calendarEventTypes = _calendarEventTypesExtractor.Extract(almaSchoolCode);
            var request = new RestRequest($"v2/{almaSchoolCode}/school/calendar/events{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            var calendarResponse = new Utf8JsonSerializer().Deserialize<CalendarEventsResponse>(response);

            Console.WriteLine($"Processing {calendarResponse.response.Count} Calendars.");

            calendarResponse.response.ForEach(c =>
            {
                c.SchoolYear = almaSchoolYears.FirstOrDefault(sy => sy.id == c.schoolYearId);
                c.EventType = calendarEventTypes.FirstOrDefault(ev => ev.id == c.eventTypeId);
            });

            return calendarResponse;
        }
    }
}
