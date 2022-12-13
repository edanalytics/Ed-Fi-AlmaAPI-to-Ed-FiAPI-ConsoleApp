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
        string TransformSrcToEdFi(string gradingPeriodsId, List<Session> almaSessions);
        Session TransformSrcToEdFi(List<Session> almaSessions, string schoolYearId);
    }
    public class SessionNameTransformer : ISessionNameTransformer
    {
        private readonly ISessionsExtractor _sessionsExtractor;

        public SessionNameTransformer(ISessionsExtractor sessionsExtractor)
        {
            _sessionsExtractor = sessionsExtractor;
        }
        public string TransformSrcToEdFi(List<Session> almaSessions, string schoolYearId, DateTime effectiveDate)
        {
            var almaSessionsForGivenSchoolYear = almaSessions.Where(x => x.schoolYearId == schoolYearId).ToList();
            var terms = new List<Term>();
            almaSessionsForGivenSchoolYear.ForEach(c => { terms.AddRange(c.terms); });
            var sessionList = terms.Where(t => t.startDate <= effectiveDate
                                              && t.endDate >= effectiveDate).ToList();

            return sessionList.FirstOrDefault().name;
        }

        public Session TransformSrcToEdFi(List<Session> almaSessions, string schoolYearId)
        {
            return almaSessions.FirstOrDefault(x => x.schoolYearId == schoolYearId);
        }

        public string TransformSrcToEdFi(string gradingPeriodsId, List<Session> almaSessions)
        {
            var terms = new List<Term>();
            almaSessions.ForEach(c => { terms.AddRange(c.terms); });
            return terms.Where(t => t.termId == gradingPeriodsId).FirstOrDefault().name;
        }
    }
}
