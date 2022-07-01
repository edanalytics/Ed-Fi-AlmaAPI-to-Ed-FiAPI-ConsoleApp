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
    public class StaffSchoolAssociationsProcessor : IProcessor
    {
        public int ExecutionOrder => 180;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStaffSchoolAssociationTransformer _staffSchoolAssociationTransformer;
        private readonly IUserRolesExtractor _userRolesExtractor;
        public StaffSchoolAssociationsProcessor(IAlmaApi almaApi,
                                                IEdFiApi edFiApi,
                                                IUserRolesExtractor userRolesExtractor,
                                                IStaffSchoolAssociationTransformer staffSchoolAssociationTransformer,
                                                ILoggerFactory logger,
                                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _staffSchoolAssociationTransformer = staffSchoolAssociationTransformer;
            _appLog = logger.CreateLogger("Staff School Associations Processor");
            _userRolesExtractor = userRolesExtractor;
        }

        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing  Staffs School Association from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students School from the source API
            var staffSchool = _apiAlma.StaffSchool.Extract(almaSchoolCode,schoolYearId);
            var userRoles = _apiAlma.UserRoles.Extract(almaSchoolCode,schoolYearId);
            
            Transform(stateSchoolId,staffSchool, almaSchoolCode, userRoles).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {staffSchool.Count} Staff School Association.");
        }

        private List<EdFiStaffSchoolAssociation> Transform(int schoolId,List<Staff> srcStaffEnrollments,string  almaSchoolCode, List<UserRole>  userRoles)
        {
            var EdfiStaffSchool = new List<EdFiStaffSchoolAssociation>();
            srcStaffEnrollments.ForEach(x => EdfiStaffSchool.Add(_staffSchoolAssociationTransformer.TransformSrcToEdFi(schoolId,x,almaSchoolCode, userRoles)));
            return EdfiStaffSchool;
        }

        private void Load(EdFiStaffSchoolAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.StaffSchoolAssociations.PostStaffSchoolAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Staffs School Association registered (Last Staff Id: {resource.StaffReference.StaffUniqueId })");
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
