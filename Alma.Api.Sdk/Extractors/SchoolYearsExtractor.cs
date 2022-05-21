using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Extractors
{
    public interface ISchoolYearsExtractor
    {
        List<SchoolYear> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class SchoolYearsExtractor : ISchoolYearsExtractor
    {
        private readonly RestClient _client;
        public SchoolYearsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        
        public List<SchoolYear> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/school-years", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            if (!string.IsNullOrEmpty(schoolYearId))
            {
                var SchoolYearList= new List<SchoolYear>();
                SchoolYearList.Add(new Utf8JsonSerializer().Deserialize<Response<SchoolYear>>(response).response);
                return SchoolYearList;
            }
            var SchoolYearResponse = new Utf8JsonSerializer().Deserialize<ListResponse<SchoolYear>>(response);
            return SchoolYearResponse.response;

        }
    }
}
