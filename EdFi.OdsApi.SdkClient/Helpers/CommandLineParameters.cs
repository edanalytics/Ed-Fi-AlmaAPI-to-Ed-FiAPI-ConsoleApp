using EdFi.AlmaToEdFi.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

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
            schoolYearFilter.Required = false;

            var awsKey = new Option<string>(
               "--awsKey",
               description: "Your Aws Key", getDefaultValue: () => "");
            awsKey.Required = false;

            var awsSecret = new Option<string>(
               "--awsSecret",
                description: "your Aws Secret", getDefaultValue: () => "");
            awsSecret.Required = false;

            var awsRegion = new Option<string>(
                "--awsRegion",
                description: "your Aws Region (e.g. us-east-1)", getDefaultValue: () => "");
            awsRegion.Required = false;

            var awsLoggingGroupName = new Option<string>(
                "--awsLoggingGroupName",
                description: "Each log stream has to belong to one log group (e.g. AlmaApi)", getDefaultValue: () => "");
            awsLoggingGroupName.Required = false;

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
