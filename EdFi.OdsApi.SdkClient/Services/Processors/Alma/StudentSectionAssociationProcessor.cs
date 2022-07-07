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
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class StudentSectionAssociationProcessor : IProcessor
    {
        public int ExecutionOrder => 120;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentSectionAssociationTransformer _studentSectionAssociationTransformer;
        public StudentSectionAssociationProcessor(IAlmaApi almaApi,
                                                  IEdFiApi edFiApi,
                                                  ILoggerFactory logger,
                                                  ILoadExceptionHandler exceptionHandler,
                                                  IStudentSectionAssociationTransformer studentSectionAssociationTransformer)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _studentSectionAssociationTransformer = studentSectionAssociationTransformer;
            _appLog = logger.CreateLogger("Student Section Association Processor");
        }



        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Student Section Association from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            // Extract - Get students Section Association from the source API
            var almaStudentSectionResponse = _apiAlma.StudentSection.Extract(almaSchoolCode,schoolYearId);
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            Transform(almaStudentSectionResponse, stateSchoolId, almaSessions).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStudentSectionResponse.Count} Student Sections Association rows .");
        }

        private List<EdFiStudentSectionAssociation> Transform(List<StudentSectionResponse> studentAttendance, int schoolId, List<Session> almaSessions)
        {
            var studentSectionAssociation = new List<EdFiStudentSectionAssociation>();
            var EdfiSections = new List<EdFiSection>();
            studentAttendance.ForEach(x => studentSectionAssociation.AddRange(_studentSectionAssociationTransformer.TransformSrcToEdFi(schoolId,x, almaSessions)));
            return studentSectionAssociation.GroupBy(x => new { x.SectionReference.SchoolYear, 
                                                                x.SectionReference.LocalCourseCode,
                                                                x.SectionReference.SchoolId,
                                                                x.SectionReference.SectionIdentifier,
                                                                x.SectionReference.SessionName,
                                                                x.StudentReference.StudentUniqueId,
                                                            })
                                                .Select(g => g.First())
                                                .ToList(); ;
        }


        private void Load(EdFiStudentSectionAssociation resource, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.SSectionAssociations.PostStudentSectionAssociationWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
                
                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Registering  Section Association for Student Id: {resource.StudentReference.StudentUniqueId}");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError( $"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}");
            }
        }
    }
}
