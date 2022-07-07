using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class SessionsProcessor : IProcessor
    {
        public int ExecutionOrder => 20;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ISessionsTransformer _sessionsTransformer;
        private readonly ICalendarNonInstructionalDaysExtractor _nonInstructionalDays;
        public SessionsProcessor(IAlmaApi almaApi, 
                                    IEdFiApi edFiApi, 
                                    IAlmaLog log, 
                                    ISessionsTransformer sessionsTransformer,
                                    ICalendarNonInstructionalDaysExtractor nonInstructionalDays,
                                     ILoggerFactory logger,
                                     ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _sessionsTransformer = sessionsTransformer;
            _nonInstructionalDays = nonInstructionalDays;
            _appLog = logger.CreateLogger("Section Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Sessions from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // Extract - Get Sessions from the source API
            var almaSessionsResponse = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            var almaNonInstructionalResponse = _nonInstructionalDays.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId, almaSessionsResponse, almaNonInstructionalResponse).ForEach(x => Load(x,almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {almaSessionsResponse.Count} Sessions.");
        }
        private List<EdFiSession> Transform(int schoolId, List<Session> srcSessions, List<CalendarEvent> almaNonInstructionalDays)
        {
            var EdfiSessions = new List<EdFiSession>();
            srcSessions.ForEach(x => EdfiSessions.AddRange(_sessionsTransformer.TransformSrcToEdFi(schoolId, x, almaNonInstructionalDays)));
            return EdfiSessions;
        }
       
        private void Load(EdFiSession resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.Sessions.PostSessionWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Sessions registered ( Last session : {resource.SessionName})");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)} :  {almaSchoolCode}/grading-periods");
            }
        }
    }
}
