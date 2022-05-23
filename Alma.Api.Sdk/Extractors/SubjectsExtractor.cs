using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;
namespace Alma.Api.Sdk.Extractors
{
    public interface ISubjectsExtractor
    {
        
        List<Subject> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class SubjectsExtractor : ISubjectsExtractor
    {
        private readonly RestClient _client;
        public SubjectsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public List<Subject> Extract(string almaSchoolCode, string schoolYearId = "")
        { //Exists any filter for School Year????
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{almaSchoolCode}/subjects{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var SubjectResponse = new Utf8JsonSerializer().Deserialize<SubjectsResponse>(response);
            return SubjectResponse.response;
        }
    }
}
