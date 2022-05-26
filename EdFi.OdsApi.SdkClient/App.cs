using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
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
            var overallStopWatch = Stopwatch.StartNew();
            var startTime = DateTime.Now;
            foreach (var school in almaSchools.response.schools)
            {
                // _processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL(school.id));
                // Checking if we need to filter endpoints by SchoolYear
                if (string.IsNullOrEmpty(_appSettings.SourceAlmaAPISettings.SchoolYearFilter))
                {
                    RunProcessors(school,string.Empty);
                }
                else
                {
                    ProcessorWithSchoolYear(school);
                }
            }
            // Process all in dependency/execution order
            //_processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL());
            overallStopWatch.Stop();
            var  endTime = DateTime.Now;
            var span = endTime.Subtract(startTime);
            _logger.LogInformation($"All done in {(overallStopWatch.ElapsedMilliseconds/1000)} seconds!/,{span.Minutes} Minutes / { span.Hours} hours");

            await Task.CompletedTask;
        }

        private void RunProcessors(School school, string schoolYearId)
        {
            foreach (var processor in _processors.OrderBy(x => x.ExecutionOrder))
            {
                ConsoleHelpers.WriteTitle($"Executing - {processor.GetType().Name}");
                processor.ExecuteETL(school.id, Convert.ToInt32(school.stateId), schoolYearId);
                // Test a single one.
                //var test1 = _processors.SingleOrDefault(x => x.GetType() == typeof(CourseProcessor));
                //test1.ExecuteETL(school.id);
            }
        }

        private void ProcessorWithSchoolYear(School school)
        {
            var schoolYearFilter = _appSettings.SourceAlmaAPISettings.SchoolYearFilter;
            var schoolYear = _schoolYearsExtractor.Extract(school.id).Where(sY => sY.name == schoolYearFilter).ToList();
            if (schoolYear.Count==0)
            {
                _logger.LogInformation($"");
                _logger.LogInformation($" *********** {school.id.ToUpper()} does not have any Record with the School Year( {schoolYearFilter }  )  *************");
                _logger.LogInformation($"");
            }
            else
            {
               var schoolYearItems = schoolYear.GroupBy(sy => sy.id)
                                        .Select(m => new Alma.Api.Sdk.Models.SchoolYear
                                        {
                                            id = m.Key
                                        }).ToList();
                foreach (var sYear in schoolYearItems)
                {
                    RunProcessors(school, sYear.id);
                }

            }
        }

    }
}
