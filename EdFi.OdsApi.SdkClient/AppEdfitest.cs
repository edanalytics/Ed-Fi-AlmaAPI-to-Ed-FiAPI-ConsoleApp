using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Processors;
using EdFi.AlmaToEdFi.Cmd.Services.Processors.AlmaAPI;
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
    public class AppEdfitest
    {
        private readonly ILogger<AppEdfitest> _logger;
        private readonly IEnumerable<IProcessor> _processors;
        private readonly AppSettings _appSettings;
        private readonly ISchoolYearsExtractor _schoolYearsExtractor;
        public AppEdfitest(IOptionsSnapshot<AppSettings> settings, ILogger<AppEdfitest> logger, IEnumerable<IProcessor> processors, ISchoolYearsExtractor schoolYearsExtractor)
        {
            _appSettings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _processors = processors;
            _schoolYearsExtractor = schoolYearsExtractor;
        }

        public async Task Run(string[] args)
        {
            _logger.LogInformation($"{DateTime.Now.ToLongDateString()} - Starting Alma API <> Ed-Fi API Sync...");

            _logger.LogInformation($"Syncing from {_appSettings.AlmaAPI.Connections.Alma.SourceConnection.Url} to {_appSettings.AlmaAPI.Connections.EdFi.TargetConnection.Url} ");

            Console.WriteLine($"There are {_processors.Count()} processors registered.");
            //  Get all schools in the district.
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

            var endTime = startTime.AddMinutes(46);
            var processors = _processors.FirstOrDefault(x => x.ExecutionOrder==-10);

            while (DateTime.Now < endTime)
            {
                _logger.LogInformation($"Started at: {startTime} / Current time :{DateTime.Now }  /  Will Finish{endTime} ");
                processors.ExecuteETL("" ,1, "");
                System.Threading.Thread.Sleep(13000);
            }

            // Process all in dependency/execution order
            //_processors.OrderBy(x => x.ExecutionOrder).ToList().ForEach(x => x.ExecuteETL());
            overallStopWatch.Stop();
               endTime = DateTime.Now;
            var span = endTime.Subtract(startTime);
            _logger.LogInformation($"All done in {(overallStopWatch.ElapsedMilliseconds/1000)} seconds!/,{span.Minutes} Minutes / { span.Hours} hours");

            await Task.CompletedTask;
        }


    }
}
