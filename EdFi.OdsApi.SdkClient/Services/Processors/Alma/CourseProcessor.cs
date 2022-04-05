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
    public class CourseProcessor : IProcessor
    {
        public int ExecutionOrder => 30;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ICourseTransformer _courseTransform;
        private readonly ILogger _appLog;
        public CourseProcessor(IAlmaApi almaApi,
                               IEdFiApi edFiApi,
                               ICourseTransformer courseTransform,
                               ILoggerFactory logger,
                               ILoadExceptionHandler exceptionHandler)
        {
            _apiEdFi = edFiApi;
            _apiAlma = almaApi;
            _exceptionHandler = exceptionHandler;
            _courseTransform = courseTransform;
            _appLog = logger.CreateLogger("Course Processor");
        }


        public void ExecuteETL(string almaSchoolCode, int stateSchoolId)
        {
            _appLog.LogInformation($"Processing Courses from School({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId)
        {
            var almaResponse = _apiAlma.Courses.Extract(almaSchoolCode);
            Transform(stateSchoolId,almaResponse).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {almaResponse.Count} courses.");
        }

        private List<EdFiCourse> Transform(int schoolId,List<Course> almaCourses)
        {
            return almaCourses.Select(course => _courseTransform.TransformSrcToEdFi(schoolId, course))
                                        .GroupBy(x => new { x.Id, x.CourseCode })
                                        .Select(g => g.First())
                                        .ToList().ToList();
        }
        private void Load(EdFiCourse resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.Courses.PostCourseWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);
                
                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Courses Registered. (Last course registered: {resource.CourseTitle})");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)}  :  /{almaSchoolCode}/courses/{resource.CourseCode}");
            }
        }
    }
}