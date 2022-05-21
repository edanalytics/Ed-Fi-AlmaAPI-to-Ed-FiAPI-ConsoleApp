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
    public class StaffProcessor : IProcessor
    {
        public int ExecutionOrder => 140;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffsTransformer _staffsTransformer;
        private readonly ILogger _appLog;
        public StaffProcessor(IAlmaApi almaApi,
                                IEdFiApi edFiApi,
                               ILoggerFactory logger,
                               IStaffsTransformer staffsTransformer,
                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffsTransformer = staffsTransformer;
            _appLog = logger.CreateLogger("Staff Processor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Staffs from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students from the source API
            var almaStaffsResponse = _apiAlma.Staff.Extract(almaSchoolCode,schoolYearId);
            Transform(almaStaffsResponse.response).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($" ");
            _appLog.LogInformation($"Processed {almaStaffsResponse.response.Count} Staffs.");
        }

        private List<EdFiStaff> Transform(List<Staff> srcStaff)
        {
            var EdfiEdFiStaffs = new List<EdFiStaff>();
            srcStaff.ForEach(x => EdfiEdFiStaffs.Add(_staffsTransformer.TransformSrcToEdFi(x)));
            return EdfiEdFiStaffs;
        }
        private void Load(EdFiStaff staff, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.Staffs.PostStaffWithHttpInfo(staff);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staffs registered( Last Staff registered: {staff.FirstName})");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, staff);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(staff)}");
            }
        }
    }
}
