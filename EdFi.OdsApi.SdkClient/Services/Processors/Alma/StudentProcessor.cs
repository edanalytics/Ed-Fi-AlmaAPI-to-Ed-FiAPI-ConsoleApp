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
    public class StudentProcessor : IProcessor
    {
        public int ExecutionOrder => 80;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentsTransformer _studentsTransformer;
        public StudentProcessor(IAlmaApi almaApi, 
                                IEdFiApi edFiApi,
                                 ILoggerFactory logger,
                                IStudentsTransformer studentsTransformer,
                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _studentsTransformer = studentsTransformer;
            _appLog = logger.CreateLogger("Student Processor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Students from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            var almaStudentResponse = _apiAlma.Students.Extract(almaSchoolCode,schoolYearId);
            Transform(almaStudentResponse.response).ForEach(x => Load(x, almaSchoolCode)); 
            ConsoleHelpers.WriteTextReplacingLastLine($"");
            _appLog.LogInformation($"Processed {almaStudentResponse.response.Count} Students.");
        }

        private List<EdFiStudent> Transform(List<Student> srcStudents)
        {
            var EdfiStudents = new List<EdFiStudent>();
            //srcStudents.ForEach(x => EdfiStudents.Add(_studentsTransformer.TransformSrcToEdFi(x)));
            foreach (var student in srcStudents)
            {
                if (student.stateId != null)
                {
                    EdfiStudents.Add(_studentsTransformer.TransformSrcToEdFi(student));
                }
                else
                {
                    _appLog.LogInformation($"INFO: Student with AlmaID: {student.id} skipped. Missing a stateID element.");
                }
            }
            return EdfiStudents;
        }
        private void Load(EdFiStudent student, string almaSchoolCode)
        {
            try
            {
                if (_apiEdFi.NeedsRefreshToken())
                    _apiEdFi.RefreshToken();

                var result = _apiEdFi.Students.PostStudentWithHttpInfo(student);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Students registered.");
                }
                
            }
            catch (Exception ex) {
                _exceptionHandler.HandleException(ex, student);
                _appLog.LogError( $"{ex.Message} Resource: {JsonConvert.SerializeObject(student)}  {almaSchoolCode}/students/{student.StudentUniqueId}");
            }
        }
    }
}
