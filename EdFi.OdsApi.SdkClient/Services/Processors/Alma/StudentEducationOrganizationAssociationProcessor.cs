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
    public class StudentEducationOrganizationAssociationProcessor : IProcessor
    {
        public int ExecutionOrder => 100;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        
        
        
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentEducationOrganizationAssociationTransformer _studentEducationOrganizationAssociation;
        public StudentEducationOrganizationAssociationProcessor(IAlmaApi almaApi,
                                                                IEdFiApi edFiApi,
                                                                ILoggerFactory logger,
                                                                IStudentEducationOrganizationAssociationTransformer studentEducationOrganizationAssociation, 
                                                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _studentEducationOrganizationAssociation = studentEducationOrganizationAssociation;
            _appLog = logger.CreateLogger("Student Education Organization Association Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Students Education Organization Association from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get  Student Education Organization Association from the source API
            var almaStudentResponse = _apiAlma.Students.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId,almaStudentResponse.response).ForEach(x => Load(x, almaSchoolCode)); 
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStudentResponse.response.Count} Student Education Organization.");
        }

        private List<EdFiStudentEducationOrganizationAssociation> Transform(int schoolId, List<Student> srcStudents)
        {
            var EdfiStudentEducationOrganizations = new List<EdFiStudentEducationOrganizationAssociation>();
            //Expanded line below.
            //srcStudents.ForEach(x =>EdfiStudentEducationOrganizations.Add(_studentEducationOrganizationAssociation.TransformSrcToEdFi(schoolId,x)));
            foreach (var x in srcStudents)
            {
                if (!String.IsNullOrEmpty(x.districtId)){
                    //If the districtId isn't null or empty handle it normally. Else output to the logger that the job was skipped.
                    EdfiStudentEducationOrganizations.Add(_studentEducationOrganizationAssociation.TransformSrcToEdFi(schoolId, x));
                } else
                {
                    _appLog.LogInformation($"INFO: Skipped StudentEducationOrganizationAssociation POST for StudentID: {x.id} because the 'DistrictID' element was null or empty.");
                }
                
            }
            return EdfiStudentEducationOrganizations;
        }

        private void Load(EdFiStudentEducationOrganizationAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.SEOrgAssociations.PostStudentEducationOrganizationAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
                RecordIndex++;
                if (RecordIndex % 10 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Student Education Organization Association : Last Student Id {resource.StudentReference.StudentUniqueId }");
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
