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
    public class StaffEducationOrganizationAssignmentAssociationProcesor : IProcessor
    {
        public int ExecutionOrder => 160;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffEducationOrganizationAssignmentTransformer _staffEducationOrganizationAssignmentTransformer;
        public StaffEducationOrganizationAssignmentAssociationProcesor(IAlmaApi almaApi, 
                                                                        IEdFiApi edFiApi,
                                                                        IAlmaLog log,
                                                                        IStaffEducationOrganizationAssignmentTransformer StaffEducationOrganizationAssignmentTransformer,
                                                                        ILoggerFactory logger,
                                                                        ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffEducationOrganizationAssignmentTransformer = StaffEducationOrganizationAssignmentTransformer;
            _appLog = logger.CreateLogger("Staff Education Organization Assignment Association Procesor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Staff Education Organization Assignments  from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students School from the source API
            var staffSchool = _apiAlma.StaffSchool.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId,staffSchool).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {staffSchool.Count} Staff School Association.");
        }

        private List<EdFiStaffEducationOrganizationAssignmentAssociation> Transform(int schoolId,List<Staff> srcStaffs)
        {
            var EdfiStaffEducationOrganization = new List<EdFiStaffEducationOrganizationAssignmentAssociation>();
            srcStaffs.ForEach(x => EdfiStaffEducationOrganization.Add(_staffEducationOrganizationAssignmentTransformer.TransformSrcToEdFi(schoolId,x)));
            return EdfiStaffEducationOrganization;
        }

        private void Load(EdFiStaffEducationOrganizationAssignmentAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.TokenNeedsToRenew())
                    _apiEdFi.RenewToken();

                var result = _apiEdFi.StaffEducationOrganizationAssignment.PostStaffEducationOrganizationAssignmentAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
               
                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staffs Education Organization Assignment Registered (Last Staff Id registered: {resource.StaffReference.StaffUniqueId })");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)} :  {almaSchoolCode}/staff/{resource.StaffReference.StaffUniqueId}");
            }
        }
    }
}
