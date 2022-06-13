using EdFi.AlmaToEdFi.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Text;
using System.Text.RegularExpressions;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
   static class CommandLineParameters
    {
        public static readonly string  _defaultValue = "";
        public static RootCommand GetCommandLineParameters()
        {
           
        // Create some options:
        var awsSourceConnectionName = new Option<string>(
             "--awsSourceConnectionName",
             description: "Aws Parameters Store Could have multiple sources.", getDefaultValue: () => _defaultValue);
            awsSourceConnectionName.AddValidator(r =>
            {
                var sourceConnectionName = "/AlmaApi/Connections/Alma/{SourceConnection1}/url \r /AlmaApi/Connections/Alma/{SourceConnection2}/url";
                var value = r.GetValueOrDefault<string>();
                if (!string.IsNullOrEmpty(value))
                    if (value.ToLower().Contains("your_source_connection_parameters") || value.Contains("<") || value.Contains(">"))
                        return $"{r.Token.Value} is not valid \r {sourceConnectionName}";
                return null;
            });
            var awsDestinationConnectionName = new Option<string>(
             "--awsDestinationConnectionName",
             description: "Aws Parameters Store Could have multiple destinations.", getDefaultValue: () => _defaultValue);
            awsDestinationConnectionName.AddValidator(r =>
            {
                var destinationConnectionName = "/AlmaApi/Connections/Edfi/{DestinationConnection1}/url \r /AlmaApi/Connections/Edfi/{DestinationConnection2}/url";

                var value = r.GetValueOrDefault<string>();
                if (!string.IsNullOrEmpty(value))
                    if (value.ToLower().Contains("your_destination_connection_parameters") || value.Contains("<") || value.Contains(">"))
                        return $"{r.Token.Value} is not valid \r {destinationConnectionName}";
                return null;
            });

            var schoolYearFilter = new Option<string>(
                "--schoolYearFilter",
                description: "if you want to filter by School Year , pass the value (e.g. 2019-2020)", getDefaultValue: () => _defaultValue);
                schoolYearFilter.AddValidator(r =>
                    {
                        var pattern = @"^\d{4}(-\d{4})$";
                        var value = r.GetValueOrDefault<string>();
                        if (!string.IsNullOrEmpty(value))
                            if (!Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                                return $"{r.Token.Value} - should be something like  2020-2021";
                        return null;
                    });

            var schoolFilter = new Option<string>(
             "--schoolFilter",
             description: "The School Information you want to request.", getDefaultValue: () => _defaultValue);
            schoolFilter.AddValidator(r =>
                {
                    var value = r.GetValueOrDefault<string>();
                    if (!string.IsNullOrEmpty(value))
                        if (value.ToLower().Contains("your_school") || value.Contains("<") || value.Contains(">"))
                            return $"{r.Token.Value} is not valid";
                    return null;
                });


            var awsRegion = new Option<string>(
                "--awsRegion",
                description: "your Aws Region (e.g. us-east-1)", getDefaultValue: () => "");
                awsRegion.AddValidator(r =>
                {
                    var value = r.GetValueOrDefault<string>();
                    if (!string.IsNullOrEmpty(value))
                        if (value.ToLower().Contains("your_aws_region") || value.Contains("<") || value.Contains(">"))
                            return $"{value} is not a valid Region";
                    return null;
                });

            var awsLoggingGroupName = new Option<string>(
                "--awsLoggingGroupName",
                description: "Each log stream has to belong to one log group (e.g. AlmaApi)", getDefaultValue: () => _defaultValue);
                awsLoggingGroupName.AddValidator(r =>
                    {
                        var value = r.GetValueOrDefault<string>();
                        if (!string.IsNullOrEmpty(value))
                            if (value.ToLower().Contains("your_log_group") || value.Contains("<") || value.Contains(">"))
                                return $"{value} is not a valid Log Group";
                        return null;
                    });

            // Add the options to a root command:
            var rootCommand = new RootCommand
                {
                    awsSourceConnectionName,
                    awsDestinationConnectionName,
                    schoolYearFilter,
                    schoolFilter,
                    awsRegion,
                    awsLoggingGroupName
                };
            rootCommand.Description = "EdFi.AlmaToEdFi.Cmd (Example of parameters :  --schoolYearFilter 2019-2020,--awsRegion us-east-1,--awsLoggingGroupName AlmaApi)";
            
            return rootCommand;
        }

        public static AppSettings CheckParameters(
            AppSettings settings,
            string awsSourceConnectionName,
            string awsDestinationConnectionName,
            string schoolYearFilter,string schoolFilter,
            string awsRegion,string awsLoggingGroupName)
        {
            //ovewrite the appsetting value with the parameter
            settings.AwsConfiguration = new AwsConfiguration();
            if (!string.IsNullOrEmpty(awsSourceConnectionName.Trim()))
                settings.AlmaAPI.Connections.SourceConnectionFilter = awsSourceConnectionName;

            if (!string.IsNullOrEmpty(awsDestinationConnectionName.Trim()))
                settings.AlmaAPI.Connections.TargetConnectionFilter = awsDestinationConnectionName;

            if (!string.IsNullOrEmpty(schoolYearFilter.Trim()))
                settings.AlmaAPI.Connections.Alma.SourceConnection.SchoolYearFilter = schoolYearFilter;

            if (!string.IsNullOrEmpty(schoolFilter.Trim()))
                settings.AlmaAPI.Connections.Alma.SourceConnection.SchoolFilter = schoolFilter;

            if (!string.IsNullOrEmpty(awsRegion.Trim()))
                settings.Logging.Region = awsRegion;

            if (!string.IsNullOrEmpty(awsLoggingGroupName.Trim()))
                settings.Logging.LogGroup = awsLoggingGroupName;
            return settings;
        }
    }
}
