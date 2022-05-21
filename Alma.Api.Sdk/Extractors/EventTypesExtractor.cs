using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Extractors
{
    public interface ICalendarEventTypesExtractor
    {
        
        List<EventTypes> Extract(string almaSchoolCode);
    }

    public class CalendarEventTypesExtractor : ICalendarEventTypesExtractor
    {
        private readonly RestClient _client;
        public CalendarEventTypesExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public List<EventTypes> Extract(string almaSchoolCode)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/school/calendar/event-types", DataFormat.Json);
            var response = _client.Get(request);

            //Deserialize JSON data
            var eventTypesResponse = new Utf8JsonSerializer().Deserialize<ListResponse<EventTypes>>(response);

            return eventTypesResponse.response;
        }
    }
}
