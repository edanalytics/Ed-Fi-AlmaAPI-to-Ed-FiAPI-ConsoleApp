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
        public static RootCommand GetCommandLineParameters()
        {
            // Create some options: 
            var schoolYearFilter = new Option<string>(
                "--schoolYearFilter",
                description: "if you want to filter by School Year , pass the value (e.g. 2019-2020)", getDefaultValue: () => "");
                schoolYearFilter.AddValidator(r =>
                {
                    string pattern = @"^\d{4}(-\d{4})$";
                    var value = r.GetValueOrDefault<string>();
                    if (!string.IsNullOrEmpty(value))
                        if (!Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase))
                            return $"{r.Token.Value} should be something like  2020-2021";
                    return null;
                });
             

            var awsKey = new Option<string>(
                        "--awsKey",
                        description: "Your Aws Key", getDefaultValue: () => "");
                        awsKey.Required = false;
                        awsKey.AddValidator(r =>
                        {
                            var value = r.GetValueOrDefault<string>();
                            if (!string.IsNullOrEmpty(value))
                                if (value.ToLower().Contains("your_aws_key") ||  value.Contains("<") ||  value.Contains(">"))
                                    return $"{value} is not a valid Key";
                            return null;
                        });

            var awsSecret = new Option<string>(
                        "--awsSecret",
                        description: "your Aws Secret", getDefaultValue: () => "");
                        awsSecret.Required = false;
                        awsSecret.AddValidator(r =>
                        {
                            var value = r.GetValueOrDefault<string>();
                            if (!string.IsNullOrEmpty(value))
                                if (value.ToLower().Contains("your_aws_secret") || value.Contains("<") || value.Contains(">"))
                                    return $"{value} is not a valid Secret";
                            return null;
                        });

            var awsRegion = new Option<string>(
                "--awsRegion",
                description: "your Aws Region (e.g. us-east-1)", getDefaultValue: () => "");
                awsRegion.Required = false;
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
                description: "Each log stream has to belong to one log group (e.g. AlmaApi)", getDefaultValue: () => "");
                awsLoggingGroupName.Required = false;
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
                    schoolYearFilter,
                    awsKey,
                    awsSecret,
                    awsRegion,
                    awsLoggingGroupName
                };
            rootCommand.Description = "EdFi.AlmaToEdFi.Cmd (Example of parameters :  --schoolYearFilter 2019-2020,--awsRegion us-east-1,--awsLoggingGroupName AlmaApi)";
            return rootCommand;
        }

        public static AppSettings CheckParameters(AppSettings settings, string schoolYearFilter,
                                                   string awsKey, string awsSecret, string awsRegion,
                                                   string awsLoggingGroupName)
        {
            //ovewrite the appsetting value with the parameter
            settings.AwsConfiguration = new AwsConfiguration();
            if (!string.IsNullOrEmpty(schoolYearFilter.Trim()))
                settings.SourceAlmaAPISettings.SchoolYearFilter = schoolYearFilter;

            if (!string.IsNullOrEmpty(awsKey.Trim()))
                settings.AwsConfiguration.AWSAccessKey = awsKey;

            if (!string.IsNullOrEmpty(awsSecret.Trim()))
                settings.AwsConfiguration.AWSSecretKey = awsSecret;

            if (!string.IsNullOrEmpty(awsRegion.Trim()))
                settings.AwsConfiguration.AWSRegion = awsRegion;

            if (!string.IsNullOrEmpty(awsLoggingGroupName.Trim()))
                settings.AwsConfiguration.AWSLoggingGroupName = awsLoggingGroupName;
            return settings;
        }
    }
}
