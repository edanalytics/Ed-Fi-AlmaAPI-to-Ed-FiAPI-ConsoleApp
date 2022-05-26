using EdFi.AlmaToEdFi.Common;
using EdFi.OdsApi.Sdk.Api.Resources;
using EdFi.OdsApi.Sdk.Client;
using Microsoft.Extensions.Options;

namespace EdFi.AlmaToEdFi.Cmd.Services.EdFi
{
    public interface IEdFiApi
    {
        SchoolsApi Schools { get; }
        StudentsApi Students { get; }
        StaffsApi Staffs { get; }
        CalendarsApi Calendars { get; }
        CalendarDatesApi CalendarDates { get; }
        CourseOfferingsApi CourseOfferings { get; }
        CoursesApi Courses { get; }
        SessionsApi Sessions { get; }
        SectionsApi Sections { get; }
        StudentEducationOrganizationAssociationsApi SEOrgAssociations { get; }
        StudentSchoolAssociationsApi SSchoolAssociations { get; }
        StudentSchoolAttendanceEventsApi SSchoolAttendance { get; }
        StudentSectionAssociationsApi SSectionAssociations { get; }
        StudentSectionAttendanceEventsApi SSectionAttendance { get; }
        StaffSectionAssociationsApi StaffSectionAssociations { get; }
        StaffSchoolAssociationsApi StaffSchoolAssociations { get; }
        StaffEducationOrganizationAssignmentAssociationsApi StaffEducationOrganizationAssignment { get; }
        StaffEducationOrganizationEmploymentAssociationsApi StaffEducationOrganizationEmployment { get; }
}

    public class EdFiApi : IEdFiApi
    {
        private readonly IAppSettings _settings;
        SchoolsApi _schoolsApi;
        StudentsApi _studentsApi;
        CalendarsApi _calendarsApi;
        CalendarDatesApi _calendarDatesApi;
        CourseOfferingsApi _courseOfferingsApi;
        CoursesApi _coursesApi;
        SessionsApi _sessionsApi;
        SectionsApi _sectionsApi { get; }
        StudentEducationOrganizationAssociationsApi _sEOrgAssociationsApi;
        StudentSchoolAssociationsApi _sSchoolAssociationsApi;
        StudentSchoolAttendanceEventsApi _sSchoolAttendanceApi;
        StudentSectionAssociationsApi _sSectionAssociationsApi;
        StudentSectionAttendanceEventsApi _sSectionAttendanceApi;
        StaffsApi _StaffsApi;
        StaffSectionAssociationsApi _staffSectionAssociations;
        StaffSchoolAssociationsApi _staffSchoolAssociations;
        StaffEducationOrganizationAssignmentAssociationsApi _staffEducationOrganizationAssignment;
        StaffEducationOrganizationEmploymentAssociationsApi _staffEducationOrganizationEmployment;
        public EdFiApi(IOptions<AppSettings> settings)
        {
            _settings = settings.Value;

            //Oauth configuration
            //var apiBaseUrl = "https://desktop-da95dja/WebApi/data/v3/ed-fi/";
            //var oauthUrl = "https://api.ed-fi.org/v5.3/api";
            //var clientKey = "";
            //var clientSecret = "";

            // TokenRetriever makes the oauth calls. It has RestSharp dependency, install via NuGet
            var tokenRetriever = new TokenRetriever(_settings.DestinationEdFiAPISettings.Url, 
                                                    _settings.DestinationEdFiAPISettings.Key, 
                                                    _settings.DestinationEdFiAPISettings.Secret);

            // Plug Oauth access token. Tokens will need to be refreshed when they expire
            var configuration = new Configuration
            {
                AccessToken = tokenRetriever.ObtainNewBearerToken(),
                BasePath = $"{_settings.DestinationEdFiAPISettings.Url}/data/v3/"
            };
            _schoolsApi = new SchoolsApi(configuration);
            _studentsApi = new StudentsApi(configuration);
            _calendarsApi = new CalendarsApi(configuration);
            _calendarDatesApi = new CalendarDatesApi(configuration);
            _courseOfferingsApi = new CourseOfferingsApi(configuration);
            _coursesApi = new CoursesApi(configuration);
            _sessionsApi= new SessionsApi(configuration);
            _sectionsApi = new SectionsApi(configuration);
            _sEOrgAssociationsApi = new StudentEducationOrganizationAssociationsApi(configuration);
            _sSchoolAssociationsApi = new StudentSchoolAssociationsApi(configuration);
            _sSchoolAttendanceApi = new StudentSchoolAttendanceEventsApi(configuration);
            _sSectionAssociationsApi = new StudentSectionAssociationsApi(configuration);
            _sSectionAttendanceApi = new StudentSectionAttendanceEventsApi(configuration);
            _StaffsApi = new StaffsApi(configuration);
            _staffSectionAssociations= new StaffSectionAssociationsApi(configuration);
            _staffSchoolAssociations= new StaffSchoolAssociationsApi(configuration);
             _staffEducationOrganizationAssignment = new StaffEducationOrganizationAssignmentAssociationsApi(configuration);
             _staffEducationOrganizationEmployment = new StaffEducationOrganizationEmploymentAssociationsApi(configuration);
        }

        public SchoolsApi Schools { get { return _schoolsApi; } }
        public StudentsApi Students { get { return _studentsApi; } }
        public CalendarsApi Calendars { get { return _calendarsApi; } }
        public CalendarDatesApi CalendarDates { get { return _calendarDatesApi; } }
        public CourseOfferingsApi CourseOfferings { get { return _courseOfferingsApi; } }
        public CoursesApi Courses { get { return _coursesApi; } }
        public SessionsApi Sessions { get { return _sessionsApi; } }
        public SectionsApi Sections { get { return _sectionsApi; } }
        public StudentEducationOrganizationAssociationsApi SEOrgAssociations { get { return _sEOrgAssociationsApi; } }
        public StudentSchoolAssociationsApi SSchoolAssociations { get { return _sSchoolAssociationsApi; } }
        public StudentSchoolAttendanceEventsApi SSchoolAttendance { get { return _sSchoolAttendanceApi; } }
        public StudentSectionAssociationsApi SSectionAssociations { get { return _sSectionAssociationsApi; } }
        public StudentSectionAttendanceEventsApi SSectionAttendance { get { return _sSectionAttendanceApi; } }
        public StaffsApi Staffs { get { return _StaffsApi; } }
        public StaffSectionAssociationsApi StaffSectionAssociations { get { return _staffSectionAssociations; } }
       public StaffSchoolAssociationsApi StaffSchoolAssociations { get { return _staffSchoolAssociations; } }
        public StaffEducationOrganizationAssignmentAssociationsApi StaffEducationOrganizationAssignment { get { return _staffEducationOrganizationAssignment; } }
        public StaffEducationOrganizationEmploymentAssociationsApi StaffEducationOrganizationEmployment { get { return _staffEducationOrganizationEmployment; } }
    }

}
