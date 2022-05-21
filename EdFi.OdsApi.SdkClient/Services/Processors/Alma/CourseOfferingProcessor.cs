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

namespace EdFi.AlmaToEdFi.Cmd.Services.Processors.Alma
{
    public class CourseOfferingProcessor : IProcessor
    {
        public int ExecutionOrder => 40;
        public int RecordIndex = 0;
        private IEdFiApi _apiEdFi;
        private IAlmaApi _apiAlma;
        private readonly ILoadExceptionHandler _exceptionHandler;
        private readonly ICourseOfferingTransformer _courseOfferingTransform;
        private readonly ILogger _appLog;
        public CourseOfferingProcessor(IAlmaApi almaApi, 
                                       IEdFiApi edFiApi,
                                       ICourseOfferingTransformer courseOfferingTransform,
                                        ILoggerFactory logger,
                                       ILoadExceptionHandler exceptionHandler)
        {
            _apiAlma = almaApi;
            _apiEdFi = edFiApi;
            _exceptionHandler = exceptionHandler;
            _courseOfferingTransform = courseOfferingTransform;
            _appLog = logger.CreateLogger("Course Offering Processor");
        }
        public void ExecuteETL(string almaSchoolCode, int stateSchoolId, string schoolYearId = "")
        {
            _appLog.LogInformation($"Processing Courses offered at School ({almaSchoolCode}) POSTS (new records and updates)...");
            ProcessPosts(almaSchoolCode, stateSchoolId,schoolYearId);
        }

        private void ProcessPosts(string almaSchoolCode, int stateSchoolId,string schoolYearId)
        {
            var almaCoursesOffered = _apiAlma.CourseOffering.Extract(almaSchoolCode,schoolYearId);
            var almaSessions = _apiAlma.Sessions.Extract(almaSchoolCode,schoolYearId);
            // Transform
            Transform(stateSchoolId, almaCoursesOffered, almaSessions).ForEach(x => Load(x, almaSchoolCode));
            ConsoleHelpers.WriteTextReplacingLastLine("");
            _appLog.LogInformation($"Processed {almaCoursesOffered.Count} courses offered.");
        }

        private List<EdFiCourseOffering> Transform(int schoolId, List<Course> almaCourses, List<Session> almaSessions)
        {
            return almaCourses.Select(course => _courseOfferingTransform.TransformSrcToEdFi(schoolId, course, almaSessions))
                                        .GroupBy(x => new { x.Id,
                                            x.CourseReference.CourseCode,
                                            x.SchoolReference.SchoolId,
                                            x.SessionReference.SchoolYear,
                                            x.SessionReference.SessionName })
                                        .Select(g => g.First())
                                        .ToList().ToList(); ;
        }

        private void Load(EdFiCourseOffering resource, string almaSchoolCode)
        {
            try
            {
                var result = _apiEdFi.CourseOfferings.PostCourseOfferingWithHttpInfo(resource);
                _exceptionHandler.HandleHttpCode(result);

                RecordIndex++;
                if (RecordIndex % 20 == 0)
                {
                    ConsoleHelpers.WriteTextReplacingLastLine($"{RecordIndex} Courses Offered Registered. (Last course registered: {resource.LocalCourseTitle})");
                }
                
            }
            catch (Exception ex)
            {
                _exceptionHandler.HandleException(ex, resource);
                _appLog.LogError($"{ex.Message} Resource: {JsonConvert.SerializeObject(resource)} :  /{almaSchoolCode}/courses/{resource.CourseReference.CourseCode}");
            }
        }
    }
}