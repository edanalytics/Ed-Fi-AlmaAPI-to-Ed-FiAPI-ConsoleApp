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
    public  class StaffSectionAssociationProcessor : IProcessor
    {
        public int ExecutionOrder => 190;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffSectionAssociationTransformer _staffSectionAssociationTransformer;
        public StaffSectionAssociationProcessor(IAlmaApi almaApi,
                                                IEdFiApi edFiApi,
                                                ILoggerFactory logger,
                                                ILoadExceptionHandler exceptionHandler,
                                                IStaffSectionAssociationTransformer staffSectionAssociationTransformer)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffSectionAssociationTransformer = staffSectionAssociationTransformer;
            _appLog = logger.CreateLogger("Staff Section Association Processor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Staff Section Association POST new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students Section Association from the source API
            var almaStaffSectionResponse = _apiAlma.StaffSection.Extract(almaSchoolCode,schoolYearId);
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            var userRoles = _apiAlma.UserRoles.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId, almaStaffSectionResponse, almaSessions, userRoles).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStaffSectionResponse.Count} Staff Sections Association.");
        }

        private List<EdFiStaffSectionAssociation> Transform(int schoolId,List<StaffSection> studentAttendance, List<Session> almaSessions, List<UserRole> userRoles)
        {
            var staffSectionAssociation = new List<EdFiStaffSectionAssociation>();
            studentAttendance.ForEach(x => staffSectionAssociation.AddRange(_staffSectionAssociationTransformer.TransformSrcToEdFi(schoolId,x, almaSessions, userRoles)));
            return staffSectionAssociation;
        }

        private void Load(EdFiStaffSectionAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.StaffSectionAssociations.PostStaffSectionAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 15 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex}  Staffs Section Association registered(Last Staff Id registered: {resource.StaffReference.StaffUniqueId})");
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
