using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Extractors
{
    public interface ICalendarNonInstructionalDaysExtractor
    {
        
        List<CalendarEvent> Extract(string almaSchoolCode);
    }

    public class CalendarNonInstructionalDaysExtractor : ICalendarNonInstructionalDaysExtractor
    {
        private readonly RestClient _client;
        public CalendarNonInstructionalDaysExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public List<CalendarEvent> Extract(string almaSchoolCode)
        {
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{almaSchoolCode}/school/calendar/events", DataFormat.Json);
            //Synchronous call
            var response = _client.Get(request);
            //Deserialize JSON data
            // Note: Alma captures only the Non Instructional Days. (At least this is what we got from the from API calls.)
            var NonInstructionalDays = new Utf8JsonSerializer().Deserialize<CalendarEventsResponse>(response);

            return NonInstructionalDays.response;
        }
    }
}
