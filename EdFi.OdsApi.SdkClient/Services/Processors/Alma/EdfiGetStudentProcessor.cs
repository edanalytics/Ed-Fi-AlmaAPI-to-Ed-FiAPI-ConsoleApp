using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.EdFi;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma;
using EdFi.AlmaToEdFi.Common;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI
{
    public class EdfiGetStudentProcessor : IProcessor
    {
        public int ExecutionOrder => -10;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private readonly ILogger _appLog;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly IStudentsTransformer _studentsTransformer;
        public EdfiGetStudentProcessor(IEdFiApi edFiApi,
                                 ILoggerFactory logger,
                                IStudentsTransformer studentsTransformer,
                                ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _exceptionHandler = exceptionHandler;
            _studentsTransformer = studentsTransformer;
            _appLog = logger.CreateLogger("EdfiGetStudentProcessor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Students Test ");
            var result = _apiEdFi.Students.GetStudents();
            result.ForEach(x => GetStudents(x));

        }

        private List<EdFiStudent> Transform(List<Student> srcStudents)
        {
            var EdfiStudents = new List<EdFiStudent>();
            srcStudents.ForEach(x => EdfiStudents.Add(_studentsTransformer.TransformSrcToEdFi(x)));
            return EdfiStudents;
        }
        private void GetStudents(EdFiStudent student)
        {
            try
            {
                if(_apiEdFi.TokenNeedsToRenew())
                    _apiEdFi.RenewToken();
                var result = _apiEdFi.Students.GetStudents();
                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Students registered (Last Student registered : {student.FirstName.Remove(1, student.FirstName.Length-1) })");
                }
                
            }
            catch (Exception ex) {
                _exceptionHandler.HandleException(ex, student);
                _appLog.LogError( $"{ex.Message} Resource: {JsonConvert.SerializeObject(student)}  /students/{student.StudentUniqueId}");
            }
        }
    }
}
