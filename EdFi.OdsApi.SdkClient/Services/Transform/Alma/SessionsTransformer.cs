using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ISessionsTransformer
    {
        List<EdFiSession> TransformSrcToEdFi(int schoolId, Session srcSessions, List<CalendarEvent> almaNonInstructionalDays);
    }

    public class SessionsTransformer: ISessionsTransformer
    {
        private readonly ILogger<SessionsTransformer> _logger;
        private readonly IDescriptorMappingService _descriptorMappingService;
        public SessionsTransformer(
            IDescriptorMappingService descriptorMappingService,
            ILogger<SessionsTransformer> logger)
        {
            _descriptorMappingService = descriptorMappingService;
            _logger = logger;
        }

        public List<EdFiSession> TransformSrcToEdFi(int schoolId, Session srcSessions, List<CalendarEvent> almaNonInstructionalDays)
        {
            var sessionList = new List<EdFiSession>();
            foreach (var gp in srcSessions.terms)
            {
                // Calculation:
                // Get total days for the grading period: (end date - start date  = total days)
                // Get the Non Instructional days count for that grading period. Get them by filtering the
                // non instructional days that fall between the [start and end date] of the term.
                // Then subtract the NonInstructional days from the total days.
                var totalDaysInGradingPeriod = (gp.endDate - gp.startDate).Days;
                var nonInstructionalDayCountWithinPeriod = almaNonInstructionalDays.Count(nid =>
                                                            nid.startDate >= gp.startDate
                                                            && nid.endDate <= gp.endDate);
                var totalInstructionalDays = totalDaysInGradingPeriod - nonInstructionalDayCountWithinPeriod;

                if (totalInstructionalDays > 0)
                {
                    sessionList.Add(new EdFiSession(null, gp.name, new EdFiSchoolReference(schoolId),
                                    new EdFiSchoolYearTypeReference(srcSessions.SchoolYear.endDate.Year), null, gp.startDate,
                                    gp.endDate, null, GetEdFiTermDescriptors(srcSessions.type.ToLower()),
                                    totalInstructionalDays, null));
                }
                else {
                    _logger.LogWarning( $"The Sesion {gp.name}(Start Date: {gp.startDate},End Date: {gp.endDate}) does not have any insrtuctional days. : /grading-periods");
                }
            }
            return sessionList;
        }
        public string GetEdFiTermDescriptors(string srcTermDescriptor)
        {
            var map = _descriptorMappingService.MappAlmaToEdFiDescriptor("TermDescriptor", srcTermDescriptor);
            if (map == null)
                map = _descriptorMappingService.MappAlmaToEdFiDescriptor("TermDescriptor", "default");
            return map;
        }

    }
}
