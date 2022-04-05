using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Net;

namespace Alma.Api.Sdk.Extractors
{
    public interface IDistrictSchoolsExtractor
    {
        
        Response<SchoolsResponse> Extract(string districtCode);
    }

    public class DistrictSchoolsExtractor : IDistrictSchoolsExtractor
    {
        private readonly RestClient _client;

        public DistrictSchoolsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public Response<SchoolsResponse> Extract(string districtCode)
        {
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{districtCode}/district/schools", DataFormat.Json);

            //Synchronous call
            var response = _client.Get(request);

            if (response.StatusCode != HttpStatusCode.OK)
                return new Response<SchoolsResponse>();

            //Deserialize JSON data
            var schoolsResponse = new Utf8JsonSerializer().Deserialize<Response<SchoolsResponse>>(response);
            return schoolsResponse;
        }
    }
}
