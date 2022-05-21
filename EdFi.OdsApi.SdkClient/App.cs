using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Extractors.Alma;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Processors;
using EdFi.AlmaToEdFi.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EdFi.AlmaToEdFi.Cmd
{
    public class App
    {
        private readonly ILogger<App> _logger;
        private readonly IAlmaApi _almaApi;
        private readonly IEnumerable<IProcessor> _processors;
        private readonly AppSettings _appSettings;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        public App(IOptions<AppSettings> settings, ILogger<App> logger, IAlmaApi almaApi, IEnumerable<IProcessor> processors, ISchoolYearsExtractor schoolYearsExtractor)
        {
            _appSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _almaApi = almaApi;
            _processors = processors;
            _schoolYearsExtractor = schoolYearsExtractor;
        }

        public async Task Run(string[] args)
        {
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()} - Starting Alma API <> Ed-Fi API Sync...");

            _logger.LogInformation($"Syncing from {_appSettings.SourceAlmaAPISettings.Url} to {_appSettings.DestinationEdFiAPISettings.Url} ");

            Console.WriteLine($"There are {_processors.Count()} processors registered.");
            //  Get all schools in the district.
            var almaSchools = _almaApi.DistrictSchools.Extract(_appSettings.SourceAlmaAPISettings.District);
                // Then for each school execute every processor.
                //ok 1-SchoolProcessor
                //ok 2-SessionsProcessor ***Total Instructional Days????****
                //ok 3-CourseProcessor *NumberOfParts??????*
                //ok 4-CourseOfferingProcessor  ***All Courses are Offered????****
                //ok 5-SectionProcessor
                //ok 6-CalendarProcessor
                //ok 7-CalendarDateProcessor
                //ok 8-StudentProcessor
                //ok 9-StudentSchoolAssociationsProcessor  ***How to get The Student Grade Level????****
                //ok 10-StudentEducationOrganizationAssociationProcessor  ***Alma Api does not have type for email****
                //11-StudentSchoolAttendanceEventProcessor ***Instructional Days Event????****
                //12-StudentSectionAssociationProcessor
                //13-StudentSectionAttendanceEventProcessor ***Alma Api does not have Student Section Attendance  ****
                //14-StaffProcessor
                //15-StaffEducationOrganizationEmploymentAssociationProcesor: ***How to get HireDate****
                //16-StaffEducationOrganizationAssignmentAssociationProcesor
                //17-StaffSectionProcessor
                //18-StaffSchoolAssociationsProcessor
                //19-StaffSectionAssociationProcessor
                
            string schoolYearId = "";
            var overallStopWatch = Stopwatch.StartNew();
            var startTime = DateTime.Now;
            foreach (var school in almaSchools.response.schools)
            {
                // _processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL(school.id));
                // Checking if we need to filter endpoints by SchoolYearId
                schoolYearId = GetSchoolYearId(_appSettings, school.id);
                if (schoolYearId == "-1")
                    continue;
                foreach (var processor in _processors.OrderBy(x => x.ExecutionOrder))
                {
                    ConsoleHelpers.WriteTitle($"Executing - {processor.GetType().Name}");
                    // Lets measure times for future performace enhancements where needed.
                    var stopWatch = Stopwatch.StartNew();
                    processor.ExecuteETL(school.id, Convert.ToInt32(school.stateId), schoolYearId);
                    stopWatch.Stop();
                    _logger.LogInformation($"{processor.GetType()} took {stopWatch.ElapsedMilliseconds} ms. \r\n");                    
                }

                // Test a single one.
                //var test1 = _processors.SingleOrDefault(x => x.GetType() == typeof(CourseProcessor));
                //test1.ExecuteETL(school.id);
            }


            // Process all in dependency/execution order
            //_processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL());
            overallStopWatch.Stop();
            var  endTime = DateTime.Now;
            var span = endTime.Subtract(startTime);
            _logger.LogInformation($"All done in {(overallStopWatch.ElapsedMilliseconds/1000)} seconds!/,{span.Minutes} Minutes / { span.Hours} hours");

            await Task.CompletedTask;
        }

        private string GetSchoolYearId(AppSettings appSettings,string schoolId)
        {
            string schoolYearId = "";
            var startDate = appSettings.SourceAlmaAPISettings.SchoolYear.StartDate;
            var endDate = appSettings.SourceAlmaAPISettings.SchoolYear.EndDate;
            // Exists StartDate && EndDate in the configuration
            if ((startDate > Convert.ToDateTime("01/01/0001")) &&
                (endDate > Convert.ToDateTime("01/01/0001")))
            {
                var schoolYear = _schoolYearsExtractor.Extract(schoolId).FirstOrDefault(sY =>
                                                                                            sY.startDate >= startDate
                                                                                            && sY.endDate <= endDate);
                if (schoolYear == null)
                {
                    _logger.LogInformation($"");
                    _logger.LogInformation($" *********** {schoolId.ToUpper()} does not have any School Year Configured ( {startDate.ToShortDateString() } - { endDate.ToShortDateString()} )  *************");
                    _logger.LogInformation($"");
                    schoolYearId ="-1";
                }
                else
                {
                    schoolYearId = schoolYear.id;
                }
            }
            return schoolYearId;
        }

    }
}
