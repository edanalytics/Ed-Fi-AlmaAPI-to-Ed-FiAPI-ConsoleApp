using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ISessionNameTransformer
    {
        string TransformSrcToEdFi(List<Session> almaSessions, string schoolYearId, DateTime effectiveDate);
    }
  public class SessionNameTransformer : ISessionNameTransformer
    {
        private readonly ISessionsExtractor _sessionsExtractor;
        
        public SessionNameTransformer(ISessionsExtractor sessionsExtractor)
        {
            _sessionsExtractor = sessionsExtractor;
        }

        public string TransformSrcToEdFi(List<Session> almaSessions, string  schoolYearId, DateTime effectiveDate)
        {
            var almaSessionsForGivenSchoolYear = almaSessions.FirstOrDefault(x=> x.schoolYearId == schoolYearId);

            var name = almaSessionsForGivenSchoolYear.terms
                            .FirstOrDefault(t => t.startDate <= effectiveDate
                                              && t.endDate >= effectiveDate).name;

            return name;
        }
    }
}
