using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;
using System.Net;

namespace Alma.Api.Sdk.Extractors
{
    public interface ISchoolExtractor
    {

        Response<School> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class SchoolExtractor : ISchoolExtractor
    {
        private readonly RestClient _client;
        public SchoolExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public Response<School> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{almaSchoolCode}/school", DataFormat.Json);

            //Synchronous call
            var response = _client.Get(request);

            //Deserialize JSON data
            var schoolsResponse = new Utf8JsonSerializer().Deserialize<Response<School>>(response);

            schoolsResponse.response.addresses = GetSchoolAddresses(almaSchoolCode);
            schoolsResponse.response.phones = GetSchoolPhones(almaSchoolCode);
            schoolsResponse.response.GradeLevels = GetGradeLevels(almaSchoolCode, schoolYearId);
            return schoolsResponse;
        }

        private List<Address> GetSchoolAddresses(string schoolCode)
        {
            var request = new RestRequest($"v2/{schoolCode}/school/contact/addresses", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Address>();

            //Deserialize JSON data
            var schoolsResponse = new Utf8JsonSerializer().Deserialize<Response<AddressessResponse>>(response);
            return schoolsResponse.response.addresses;
        }
        private List<Phone> GetSchoolPhones(string schoolCode)
        {
            var request = new RestRequest($"v2/{schoolCode}/school/contact/phones", DataFormat.Json);

            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Phone>();

            //Deserialize JSON data
            var schoolsResponse = new Utf8JsonSerializer().Deserialize<Response<PhonesResponse>>(response);
            return schoolsResponse.response.phones;
        }
        private List<GradeLevel> GetGradeLevels(string schoolCode,string schoolYearId = "")
        {
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{schoolCode}/grade-levels{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<GradeLevel>();

            //Deserialize JSON data
            var gradeLevelsResponse = new Utf8JsonSerializer().Deserialize<GradeLevelsResponse>(response);
            return gradeLevelsResponse.response;
        }
    }
}
