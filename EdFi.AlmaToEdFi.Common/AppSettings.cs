using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Common
{
    public interface IAppSettings    {
        public AlmaAPI AlmaAPI { get; set; }
    }
    public class AlmaAPI
    {
        public string ParameterStoreProvider { get; set; } = "";
        public Connections Connections { get; set; }
    }
    public class Connections
    {
        public string  SourceConnectionFilter { get; set; } = "";
        public string TargetConnectionFilter { get; set; } = "";
        public Alma Alma { get; set; }
        public EdFi EdFi { get; set; }
    }
    public class Alma
    {
        public SourceConnection SourceConnection { get; set; }
    }
    public class EdFi
    {
        public TargetConnection TargetConnection { get; set; }
        public DateTime RefreshTokenAt { get; set; }// minutes to renew session
        public Double RefreshTokenIn { get; set; }// minutes to renew session
    }

    public class SourceConnection : ApiConfig
    {
        public string Name { get; set; }
        public string District { get; set; }
        public string SchoolYearFilter { get; set; }
        public string SchoolFilter { get; set; }
    }

    public class TargetConnection : ApiConfig
    {
        public string Name { get; set; }
        public string DestinationLocalEducationAgencyId { get; set; }


    }
    public class AppSettings : IAppSettings
    {
        
        public int StartWithProcessor { get; set; }
        public Logging Logging { get; set; }
        public AwsConfiguration AwsConfiguration { get; set; }
        public AlmaAPI AlmaAPI { get; set; }

    }
    public class AwsConfiguration
    {
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
    }
    public abstract class ApiConfig
    {
        public string Url { get; set; }
        public string Key { get; set; }
        public string Secret { get; set; }
    }

    public class AlmaApiSettings : ApiConfig
    {
        public string District { get; set; }
        public string SchoolYearFilter { get; set; }
        public string SchoolFilter { get; set; }
        
    }

    public class EdFiApiSettings : ApiConfig
    {
        public int DestinationLocalEducationAgencyId { get; set; }
    }

    public class LogLevel
    {
        public string Default { get; set; }
        public string System { get; set; }
        public string Microsoft { get; set; }
    }

    public class File
    {
        public string Path { get; set; }
        public bool Append { get; set; }
        public string MinLevel { get; set; }
        public int FileSizeLimitBytes { get; set; }
        public int MaxRollingFiles { get; set; }
    }

    public class Logging
    {
        public LogLevel LogLevel { get; set; }
        public File File { get; set; }
        public string  Region { get; set; }
        public string LogGroup { get; set; }
        public string LogStreamNamePrefix { get; set; }
        public string LoggingProvider { get; set; }
        
    }

}
