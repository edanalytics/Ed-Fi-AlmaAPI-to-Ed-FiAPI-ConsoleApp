using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace EdFi.AlmaToEdFi.Cmd.Helpers
{
   public static class ParametersStoreMapping
    {
        const string SETTINGS_ALMAAPI = "Settings:AlmaAPI:";
        const string SOURCE_PREFIX = "Connections:Alma:";
        const string TARGET_PREFIX = "Connections:EdFi:";
        const string SOURCE = "SourceConnection";
        const string TARGET = "TargetConnection";
        const string SUFIX = ":";
        public static IConfiguration AddAlmaCustomParameters(this IConfiguration configuration,string SourceConnectionName,string TargetConnectionName)
        {
            var customedParameter = "";
            //AWS Parameter store could have with multiple sources and destinations configurations, so we need to filter by  SourceConnectionName && TargetConnectionName
            var firstOrDefaultSourceGroup = GetFirstGroupParameterItems( configuration, SOURCE_PREFIX, SourceConnectionName);
            var firstOrDefaultTargetGroup = GetFirstGroupParameterItems(configuration, TARGET_PREFIX, TargetConnectionName);

            if (firstOrDefaultSourceGroup.Count < 1)
                ExitApplication($"\rSource Connection ({SourceConnectionName}) not found in your AWS Parameter Store.  Create a collection or switch the configuration(appsettings.json) to  'ParameterStoreProvider':'appSettings'");
            if (firstOrDefaultTargetGroup.Count < 1)
                ExitApplication($"\rDestination Connection ({TargetConnectionName}) not found in your AWS Parameter Store.  Create a collection or switch the configuration(appsettings.json) to   'ParameterStoreProvider':'appSettings'");
            //custom the parameter
            firstOrDefaultSourceGroup.ForEach(item => {
                customedParameter = GetCustomedParameter(item.Key);
                configuration.ConfigureCustomSection(customedParameter, item.Value);
            });

            firstOrDefaultTargetGroup.ForEach(item => {
                customedParameter = GetCustomedParameter(item.Key);
                configuration.ConfigureCustomSection(customedParameter, item.Value);
            });

            return configuration;
        }

        public static IConfiguration ConfigureCustomSection(this IConfiguration configuration, string key, string value)
        {
            configuration.GetSection(key).Value = value;
            return configuration;
        }


        private static string GetCustomedParameter(string parameterName)
        {
            //AlmaAPI:Connections:Alma:SourceConnectionName:0:Url
            var customParameterName = "";
            switch (parameterName)
            {
                case
                var cP when parameterName.StartsWith(SOURCE_PREFIX) && parameterName.Contains(SUFIX):
                    customParameterName = GetCustomParameterName(parameterName, SOURCE_PREFIX, SUFIX, SOURCE);
                    break;
                case
                var cP when parameterName.StartsWith(TARGET_PREFIX) && parameterName.Contains(SUFIX):
                    customParameterName = GetCustomParameterName(parameterName, TARGET_PREFIX, SUFIX, TARGET);
                    break;
                default:
                    customParameterName = parameterName;
                    break;
            }
            return customParameterName;

        }


        public static List<KeyValuePair<string,string>> GetFirstGroupParameterItems(this IConfiguration configuration, string prefix,string ConnectionName)
        {
            //Needs to filter by ConnectionName
            var ParameterItems = new List<KeyValuePair<string, string>>();
            if (string.IsNullOrEmpty(ConnectionName))
            {
                ParameterItems = configuration.AsEnumerable().Where(t => (t.Key.StartsWith(prefix)) && (!string.IsNullOrEmpty(t.Value))).ToList();
                if (ParameterItems.Count>1)
                {
                    ConnectionName = GetConnectionNameParameter(ParameterItems.FirstOrDefault().Key, prefix,SUFIX);
                    ParameterItems = ParameterItems.Where(t => t.Key.Contains($":{ConnectionName}:")).ToList();
                }
            }               
            else
                ParameterItems = configuration.AsEnumerable().Where(t => t.Key.StartsWith(prefix) && (!string.IsNullOrEmpty(t.Value))  && t.Key.Contains($":{ConnectionName}:")).ToList();
            return ParameterItems;
        }

        private static string GetConnectionNameParameter(string parameterName, string prefix,string sufix="")
        {
            return Regex.Match(parameterName, $@"{prefix}(.+?)(?={sufix})").Groups[1].Value;
        }

        private static string GetCustomParameterName(string parameterName, string prefix, string sufix, string sourceOrTarget)
        {
            //Needs to replace the current source Or Target with SourceConnection,TargetConnection
            var conectionName = GetConnectionNameParameter(parameterName,prefix,sufix);
            if(!string.IsNullOrEmpty(conectionName))
                return $"{SETTINGS_ALMAAPI}{parameterName.Replace($"{conectionName}", sourceOrTarget)}";
            return parameterName;
        }

        public static void ExitApplication(string message)
        {
            try
            {
                throw new ApplicationException($"{message}");
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.Write(e.Message);
                Console.ResetColor();
                Environment.Exit(0);
            }
        }
    }
}
