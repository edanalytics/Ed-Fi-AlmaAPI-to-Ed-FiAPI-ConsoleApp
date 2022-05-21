using Alma.Api.Sdk.Extractors.Alma;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NReco.Logging.File;
using System;
using System.Collections.Generic;
using System.IO;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.Alma
{
    public class CalendarDateProcessor : IProcessor
    {
        public int ExecutionOrder => 70;
        public int RecordIndex =0;
        private IAlmaApi _apiAlma;
        private readonly IEdFiApi _edFiApi;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ICalendarDateTransformer _calendarDateTransformer;
        private readonly ILogger _appLog;
        public CalendarDateProcessor(IEdFiApi edFiApi,
                                     IAlmaApi almaApi,
                                     ICalendarDateTransformer calendarDateTransformer,
                                     ILoggerFactory logger,
        ILoadExceptionHandler exceptionHandler)
        {
            _apiAlma = almaApi;
            _edFiApi = edFiApi;
            _exceptionHandler = exceptionHandler;
            _calendarDateTransformer = calendarDateTransformer;
            _appLog = logger.CreateLogger("Calendar Date Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Calendar dates from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            //Check if schoolYearId is not emty
            var almaResponse = _apiAlma.SchoolCalendarEvents.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId, almaSchoolCode, almaResponse.response).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {almaResponse.response.Count} Calendar Dates.");
        }

        private List<EdFiCalendarDate> Transform(int schoolId, string almaSchoolCode, List<CalendarEvent> almaCalendarEvents)
        {
            return _calendarDateTransformer.TransformSrcToEdFi(schoolId, almaSchoolCode, almaCalendarEvents);
        }
        private void Load(EdFiCalendarDate resource, string almaSchoolCode)
        {
            try
            {
                var result = _edFiApi.CalendarDates.PostCalendarDateWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    resource.CalendarEvents.ForEach(ev => {
                        var eventName = ev.CalendarEventDescriptor.Split("#");
                        ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Calendar Dates registered ( Last calendar date Registered: {resource.Date.Value.ToShortDateString() }-{eventName[1]})");
                    });
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}  : {almaSchoolCode}/school/calendar/events/{resource.Id}");
            }
        }
    }
}
