using System;
using System.Collections.Generic;
using System.Text;
using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;

namespace Alma.Api.Sdk.Extractors
{
    public interface IStudentsGradeLevelExtractor
    {

        StudentsGradeLevels Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StudentsGradeLevelExtractor : IStudentsGradeLevelExtractor
    {
        private readonly RestClient _client;
        public StudentsGradeLevelExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }
        public StudentsGradeLevels Extract(string almaSchoolCode, string schoolYearId = "")
        {   //Exists any filter for School Year????
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{almaSchoolCode}/students/grade-levels{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var StudentGradeLevelsResponse = new Utf8JsonSerializer().Deserialize<Response<StudentsGradeLevels>>(response);
            return StudentGradeLevelsResponse.response;
        }
    }
}
