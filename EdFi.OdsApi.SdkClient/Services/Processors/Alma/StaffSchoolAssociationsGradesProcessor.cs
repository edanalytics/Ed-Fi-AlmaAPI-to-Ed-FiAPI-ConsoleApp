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

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.Alma
{
    public class StaffSchoolAssociationsGradesProcessor : IProcessor
    {
        public int ExecutionOrder => 181;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffSchoolAssociationsGradesTransformer _staffSchoolAssociationTransformer;
        public StaffSchoolAssociationsGradesProcessor(IAlmaApi almaApi,
                                                IEdFiApi edFiApi,
                                                IStaffSchoolAssociationsGradesTransformer staffSchoolAssociationTransformer,
                                                ILoggerFactory logger,
                                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffSchoolAssociationTransformer = staffSchoolAssociationTransformer;
            _appLog = logger.CreateLogger("Staff School Associations Grades Processor");
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId)
        {
            _appLog.LogInformation($"Processing  Staffs  grades from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students School from the source API
            var staffSchool = _apiAlma.StaffSection.Extract(almaSchoolCode);
            var userRoles = _apiAlma.UserRoles.Extract(almaSchoolCode);

            Transform(stateSchoolId, staffSchool, almaSchoolCode, userRoles).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {staffSchool.Count} Staff School Association.");
        }

        private List<EdFiStaffSchoolAssociation> Transform(int schoolId, List<StaffSection> srcStaffSchool, string almaSchoolCode, List<UserRole> userRoles)
        {
            var EdfiStaffSchool = new List<EdFiStaffSchoolAssociation>();
            srcStaffSchool.ForEach(x => EdfiStaffSchool.AddRange(_staffSchoolAssociationTransformer.TransformSrcToEdFi(schoolId, x, userRoles)));
            return EdfiStaffSchool;
        }

        private void Load(EdFiStaffSchoolAssociation resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.StaffSchoolAssociations.PostStaffSchoolAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staffs Grades registered (Last Staff Id registered: {resource.StaffReference.StaffUniqueId })");
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