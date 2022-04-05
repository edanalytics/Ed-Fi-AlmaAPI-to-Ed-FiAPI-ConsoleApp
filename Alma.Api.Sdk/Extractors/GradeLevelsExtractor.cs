using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Extractors
{
    public interface IGradeLevelsExtractor
    {
        
        List<GradeLevel> Extract(string almaSchoolCode);
    }

    public class GradeLevelsExtractor : IGradeLevelsExtractor
    {
        private readonly RestClient _client;
        public GradeLevelsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public List<GradeLevel> Extract(string almaSchoolCode)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/grade-levels", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var gradeLevelsResponse = new Utf8JsonSerializer().Deserialize<GradeLevelsResponse>(response);
            return gradeLevelsResponse.response;
        }
    }
}
