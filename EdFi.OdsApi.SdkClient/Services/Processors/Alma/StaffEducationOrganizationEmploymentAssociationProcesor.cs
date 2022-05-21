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
    public  class StaffEducationOrganizationEmploymentAssociationProcesor : IProcessor
    {
        public int ExecutionOrder => 150;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffEducationOrganizationEmploymentAssociationTransformer _staffEducationOrganizationEmploymentTransformer;
        public StaffEducationOrganizationEmploymentAssociationProcesor(IAlmaApi almaApi,
            IEdFiApi edFiApi, ILoggerFactory logger,
            IStaffEducationOrganizationEmploymentAssociationTransformer staffEducationOrganizationEmploymentTransformer, 
            ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffEducationOrganizationEmploymentTransformer = staffEducationOrganizationEmploymentTransformer;
            _appLog = logger.CreateLogger("Staff Education Organization Employment Association Procesor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Staff Education Organization Employment Association from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students School from the source API
            var staffSchool = _apiAlma.StaffSchool.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId,staffSchool).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($" ");
            _appLog.LogInformation($"Processed {staffSchool.Count} Staff Education Organization Employment Association.");
        }

        private List<EdFiStaffEducationOrganizationEmploymentAssociation> Transform(int schoolId ,List<Staff> srcStaffs)
        {
            var EdfiStaffEducationOrganizationEmployment = new List<EdFiStaffEducationOrganizationEmploymentAssociation>();
            srcStaffs.ForEach(x => EdfiStaffEducationOrganizationEmployment.Add(_staffEducationOrganizationEmploymentTransformer.TransformSrcToEdFi(schoolId,x)));
            return EdfiStaffEducationOrganizationEmployment;
        }

        private void Load(EdFiStaffEducationOrganizationEmploymentAssociation resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.StaffEducationOrganizationEmployment.PostStaffEducationOrganizationEmploymentAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staffs Education Organization Employment Association registered (Last Staff Id registered:{resource.StaffReference.StaffUniqueId })");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
            }
        }
    }
}