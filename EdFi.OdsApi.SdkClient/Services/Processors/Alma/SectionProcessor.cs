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
    public class SectionProcessor : IProcessor
    {
        public int ExecutionOrder => 50;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ISectionTransformer _sectionTransformer;
        private readonly ILogger _appLog;
        public SectionProcessor(IAlmaApi almaApi,
                                IEdFiApi edFiApi,
                                ISectionTransformer sectionTransformer,
                                ILoggerFactory logger,
                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _sectionTransformer = sectionTransformer;
            _appLog = logger.CreateLogger("Section Processor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId)
        {
            _appLog.LogInformation($"Processing Sections from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId)
        {
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode);
            var almaSections = _apiAlma.Sections.Extract(almaSchoolCode);
            Transform(almaSections, stateSchoolId, almaSessions).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {almaSections.Count} Sections.");
        }
        private List<EdFiSection> Transform(List<Section> almaSections, int schoolId, List<Session> almaSessions)
        {
            return _sectionTransformer.TransformSrcToEdFi(schoolId, almaSections, almaSessions);
        }

        private void Load(EdFiSection resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.Sections.PostSectionWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Sections registered ( Last section registered: {resource.SectionName})");
                }                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogInformation($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)} : {almaSchoolCode}/classes/{resource.SectionIdentifier}");
            }
        }
    }
}