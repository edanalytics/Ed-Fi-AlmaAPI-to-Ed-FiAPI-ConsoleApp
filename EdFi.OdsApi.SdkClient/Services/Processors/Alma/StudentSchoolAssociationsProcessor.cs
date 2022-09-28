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
    public class StudentSchoolAssociationsProcessor : IProcessor
    {
        public int ExecutionOrder => 90;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        
        
        
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentSchoolAssociationsTransformer _studentSchoolAssociationsTransformer;
        public StudentSchoolAssociationsProcessor(IAlmaApi almaApi,
                                                    IEdFiApi edFiApi,
                                                    ILoggerFactory logger,
                                                    IStudentSchoolAssociationsTransformer studentSchoolAssociationsTransformer,
                                                    ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _studentSchoolAssociationsTransformer = studentSchoolAssociationsTransformer;
            _appLog = logger.CreateLogger("Student School Associations Processor");
        }



        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Student School Associations from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // TODO: Change to ALMA once we have access to their API.
            // Extract - Get students School from the source API
            var almaStudentSchoolResponse = _apiAlma.StudentSchool.Extract(almaSchoolCode,schoolYearId);
            Transform(stateSchoolId, schoolYearId, almaStudentSchoolResponse).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStudentSchoolResponse.Count} Student School Association.");
        }

        private List<EdFiStudentSchoolAssociation> Transform(int schoolId, string schoolYearId, List<Enrollment> srcStudentEnrollments)
        {
            var EdfiStudentEnroll = new List<EdFiStudentSchoolAssociation>();
            srcStudentEnrollments.ForEach(x => EdfiStudentEnroll.Add(_studentSchoolAssociationsTransformer.TransformSrcToEdFi(schoolId, schoolYearId, x)));
            return EdfiStudentEnroll;
        }

        private void Load(EdFiStudentSchoolAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.SSchoolAssociations.PostStudentSchoolAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Student Enrollments registered(Last Student Id registered {resource.StudentReference.StudentUniqueId})");
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
