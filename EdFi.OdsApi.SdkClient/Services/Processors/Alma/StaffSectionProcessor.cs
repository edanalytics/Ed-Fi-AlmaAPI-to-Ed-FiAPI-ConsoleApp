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
namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.Alma
{
    public class StaffSectionProcessor : IProcessor
    {
        public int ExecutionOrder => 170;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffSectionTransformer _sectionTransformer;
        public StaffSectionProcessor(IAlmaApi almaApi,
                                IEdFiApi edFiApi,
                                ILoggerFactory logger,
                                IStaffSectionTransformer sectionTransformer,
                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _sectionTransformer = sectionTransformer;
            _appLog = logger.CreateLogger("Staff Section Processor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Sections from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get Sections from the source API
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            var almaResponse = _apiAlma.StaffSection.Extract(almaSchoolCode,schoolYearId);
            Transform(almaResponse, stateSchoolId, almaSessions).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaResponse.Count} Sections.");
        }
        private List<EdFiSection> Transform(List<StaffSection> srcSections, int schoolId, List<Session> almaSessions)
        {
            var EdfiSections = new List<EdFiSection>();
            srcSections.ForEach(x => EdfiSections.AddRange(_sectionTransformer.TransformSrcToEdFi(schoolId, x, almaSessions)));
            return EdfiSections;
        }

        private void Load(EdFiSection resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.Sections.PostSectionWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
                
                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staff Sections registered(Last Section registered: {resource.SectionName})");
                }
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogInformation($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
            }
        }
    }
}