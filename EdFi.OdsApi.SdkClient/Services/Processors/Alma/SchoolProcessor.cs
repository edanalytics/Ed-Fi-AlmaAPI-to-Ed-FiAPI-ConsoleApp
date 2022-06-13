using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.AlmaToEdFi.Common;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NReco.Logging.File;
using System;
using System.IO;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class SchoolProcessor : IProcessor
    {
        public int ExecutionOrder => 10;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ISchoolTransformer _schoolTransformer;
        private readonly IAppSettings _settings;
        private readonly ILogger _appLog;

        public SchoolProcessor(IOptions<AppSettings> settings,
                               IAlmaApi almaApi, 
                               IEdFiApi edFIApi,
                               ISchoolTransformer schoolTransformer,
                               ILoggerFactory logger,
                               ILoadExceptionHandler exceptionHandler)
        {
            _settings = settings.Value;
            _apiEdFi =  edFIApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _schoolTransformer = schoolTransformer;
            _appLog = logger.CreateLogger("School Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // Extract - Get School from the source API                
            var srcResponse = _apiAlma.School.Extract(almaSchoolCode,schoolYearId);

            // Transform & Load - Transform Load all the Alma records into the destination Ed-Fi API
            srcResponse.response.districtId = _settings.AlmaAPI.Connections.EdFi.TargetConnection.DestinationLocalEducationAgencyId.ToString();
            Load(TransformFromAlmaToEdFi(srcResponse.response), almaSchoolCode);
            _appLog.LogInformation($"Processed school: { almaSchoolCode}.");
        }

        private EdFiSchool TransformFromAlmaToEdFi(School srcSchools)
        {
            return _schoolTransformer.TransformSrcToEdFi(srcSchools);
        }

        private void Load(EdFiSchool resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.Schools.PostSchoolWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
            }
            catch (Exception ex)
            {
                
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogInformation($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}  :  {almaSchoolCode}/school");
            }
        }

       
    }
}
