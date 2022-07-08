using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using Microsoft.Extensions.Logging;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Api.Sdk.Extractors
{
    public interface ISessionsExtractor
    {
        List<Session> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class SessionsExtractor : ISessionsExtractor
    {
        private readonly RestClient _client;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        public SessionsExtractor(IAlmaRestClientConfigurationProvider client, 
                                 ISchoolYearsExtractor schoolYearsExtractor)
        {
            _client = client.GetRestClient();
            _schoolYearsExtractor = schoolYearsExtractor;
        }
        public List<Session> Extract(string almaSchoolCode, string schoolYearId = "")
        {
            var almaSchoolYears = _schoolYearsExtractor.Extract(almaSchoolCode);
            // NOTE: In Alma they have a different way of looking at Terms and Sessions.
            // They are calling these grading-periods.

            //Alma Api not works with schoolYearId filter
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolYearId = $"?schoolYearId={schoolYearId}";
            var request = new RestRequest($"v2/{almaSchoolCode}/grading-periods{schoolYearId}", DataFormat.Json);
            var response = _client.Get(request);
            //Deserialize JSON data
            var schoolGradingPeriods = new Utf8JsonSerializer().Deserialize<SessionsResponse>(response);
            if (!string.IsNullOrEmpty(schoolYearId))
                schoolGradingPeriods.response = schoolGradingPeriods.response.Where(gp=>gp.schoolYearId== schoolYearId).ToList();

            schoolGradingPeriods.response.ForEach(gP =>
            {
                gP.SchoolYear = almaSchoolYears.SingleOrDefault(x => x.id == gP.schoolYearId);
            });
            return schoolGradingPeriods.response;
        }
    }
}
