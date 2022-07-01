using Alma.Api.Sdk.Extractors.Alma;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.Alma
{
    public class CalendarProcessor : IProcessor
    {
        public int ExecutionOrder => 60;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ICalendarTransformer _calendarTransformer;
        private readonly ILogger _appLog;
        public CalendarProcessor(IAlmaApi almaApi,
                                 IEdFiApi edFiAPI,
                                 ICalendarTransformer calendarTransformer,
                                 ILoggerFactory logger,
                                 ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiAPI;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _calendarTransformer = calendarTransformer;
            _appLog = logger.CreateLogger("Calendar Processor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Calendars from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            var almaResponse = _apiAlma.SchoolCalendarEvents.Extract(almaSchoolCode,schoolYearId);
            var edfiCalendars = Transform(stateSchoolId, almaSchoolCode, almaResponse.response);
            edfiCalendars.ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {edfiCalendars.Count} Calendars.");
        }

        private List<EdFiCalendar> Transform(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents)
        {
            return _calendarTransformer.TransformSrcToEdFi(schoolId, almaSchoolCode, almaCalendarEvents);
        }

        private void Load(EdFiCalendar resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.TokenNeedsToRenew())
                    _apiEdFi.RenewToken();

                var result = _apiEdFi.Calendars.PostCalendarWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Calendars Registered,(Last calendar registered -> School year : {resource.SchoolYearTypeReference.SchoolYear } to School {almaSchoolCode})");
                }
            }
            catch (Exception ex) {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)} :  {almaSchoolCode}/school/calendar/events/{resource.Id}");
            }
        }
    }
}
