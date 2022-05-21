using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;

namespace Alma.Api.Sdk.Extractors
{
    public interface IUserRolesExtractor
    {
        
        List<UserRole> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class UserRolesExtractor : IUserRolesExtractor
    {
        private readonly RestClient _client;
        public UserRolesExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public List<UserRole> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/user-roles", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var eventTypesResponse = new Utf8JsonSerializer().Deserialize<Response<UserRoleResponse>>(response);
            return eventTypesResponse.response.userRoles;
        }
    }
}