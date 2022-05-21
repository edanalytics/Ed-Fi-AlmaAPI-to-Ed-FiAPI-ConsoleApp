using System;

namespace EdFi.AlmaToEdFi.Common
{
    public interface IAppSettings
    {
        EdFiApiSettings DestinationEdFiAPISettings { get; set; }
        AlmaApiSettings SourceAlmaAPISettings { get; set; }
    }

    public class AppSettings : IAppSettings
    {
        public AlmaApiSettings SourceAlmaAPISettings { get; set; }
        public EdFiApiSettings DestinationEdFiAPISettings { get; set; }
        public Logging Logging { get; set; }
        public AwsConfiguration AwsConfiguration { get; set; }
    }
    public class AwsConfiguration
    {
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
        public string AWSRegion { get; set; }
        public string AWSLoggingGroupName { get; set; }
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
        public SchoolYear SchoolYear { get; set; }
    }

    public class EdFiApiSettings : ApiConfig
    {
        public int DestinationLocalEducationAgencyId { get; set; }
    }

    public class SchoolYear
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
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
    }

}
