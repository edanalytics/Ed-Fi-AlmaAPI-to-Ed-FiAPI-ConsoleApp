using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;

namespace Alma.Api.Sdk.Extractors
{
    public interface IStudentsEnrollmentsExtractor
    {
        StudentsEnrollment Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StudentsEnrollmentsExtractor : IStudentsEnrollmentsExtractor
    {
        private readonly RestClient _client;
        public StudentsEnrollmentsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public StudentsEnrollment Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/enrollment", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var enrollmentResponse = new Utf8JsonSerializer().Deserialize<Response<StudentsEnrollment>>(response);
            return enrollmentResponse.response;
        }
    }
}
