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
        public App(IOptionsSnapshot<AppSettings> settings, ILogger<App> logger, IAlmaApi almaApi, IEnumerable<IProcessor> processors, ISchoolYearsExtractor schoolYearsExtractor)
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

            _logger.LogInformation($"Syncing from {_appSettings.AlmaAPI.Connections.Alma.SourceConnection.Url} to {_appSettings.AlmaAPI.Connections.EdFi.TargetConnection.Url} ");

            Console.WriteLine($"There are {_processors.Count()} processors registered.");
            //  Get all schools in the district.
            var almaSchools = _almaApi.DistrictSchools.Extract(_appSettings.AlmaAPI.Connections.Alma.SourceConnection.District);
            var schoolFilter = _appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolFilter;
            // Then for each school execute every processor.
            //ok 10-SchoolProcessor
            //ok 20-SessionsProcessor ***Total Instructional Days????****
            //ok 30-CourseProcessor *NumberOfParts??????*
            //ok 40-CourseOfferingProcessor  ***All Courses are Offered????****
            //ok 50-SectionProcessor
            //ok 60-CalendarProcessor
            //ok 70-CalendarDateProcessor
            //ok 80-StudentProcessor
            //ok 90-StudentSchoolAssociationsProcessor  ***How to get The Student Grade Level????****
            //ok 100-StudentEducationOrganizationAssociationProcessor  ***Alma Api does not have type for email****
            //110-StudentSchoolAttendanceEventProcessor ***Instructional Days Event????****
            //120-StudentSectionAssociationProcessor
            //130-StudentSectionAttendanceEventProcessor ***Alma Api does not have Student Section Attendance  ****
            //140-StaffProcessor
            //150-StaffEducationOrganizationEmploymentAssociationProcesor: ***How to get HireDate****
            //160-StaffEducationOrganizationAssignmentAssociationProcesor
            //170-StaffSectionProcessor
            //180-StaffSchoolAssociationsProcessor
            //190-StaffSectionAssociationProcessor
            var overallStopWatch = Stopwatch.StartNew();
            var startTime = DateTime.Now;
            if (!string.IsNullOrEmpty(schoolFilter))
                almaSchools.response.schools = almaSchools.response.schools.Where(s=>s.id== schoolFilter || s.name == schoolFilter).ToList();
            if (almaSchools.response.schools.Count < 1)
                _logger.LogInformation($" ********** No School was found that matches [{schoolFilter}]*********");
            foreach (var school in almaSchools.response.schools)
            {
                // _processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL(school.id));
                // Checking if we need to filter endpoints by SchoolYear
                if (string.IsNullOrEmpty(_appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter))
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
            var processors = _processors.Where(x => x.ExecutionOrder > 0).ToList();
            if (_appSettings.StartWithProcessor > 10)
                processors = processors.Where(x => x.ExecutionOrder >= _appSettings.StartWithProcessor).ToList();

            foreach (var processor in processors.OrderBy(x => x.ExecutionOrder))
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
            var schoolYearFilter = _appSettings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter;
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
                _logger.LogInformation($" *********** {school.id.ToUpper()} Processing records for the {schoolYearFilter } School Year *************");
                foreach (var sYear in schoolYearItems)
                {
                    RunProcessors(school, sYear.id);
                }

            }
        }

    }
}
