namespace Alma.Api.Sdk.Extractors.Alma
{
    public interface IAlmaApi
    {
        IStudentAttendanceExtractor Attendance { get; }
        ICalendarEventsExtractor SchoolCalendarEvents { get; }
        ICoursesExtractor Courses { get; }
        ICourseOfferingExtractor CourseOffering { get; }
        IDistrictSchoolsExtractor DistrictSchools { get; }
        ISchoolExtractor School { get; }
        ISectionsExtractor Sections { get; }
        ISessionsExtractor Sessions { get; }
        IStaffsExtractor Staff { get; }
        IStudentsExtractor Students { get; }
        IStudentSchoolAssociationExtractor StudentSchool { get; }
        IStaffSchoolAssociationExtractor StaffSchool { get; }
        IStudentSectionAssociationExtractor StudentSection { get; }
        IStaffSectionAssociationExtractor StaffSection { get; }
        IUserRolesExtractor UserRoles { get; }
    }

    public class AlmaApi : IAlmaApi
    {
        private readonly IDistrictSchoolsExtractor _districtSchoolsExtractor;
        private readonly ISchoolExtractor _schoolExtractor;
        private readonly ISessionsExtractor _sessionsExtractor;
        private readonly ICalendarEventsExtractor _calendarsExtractor;
        private readonly ICoursesExtractor _coursesExtractor;
        private readonly ICourseOfferingExtractor _courseOfferingExtractor;
        private readonly ISectionsExtractor _sectionsExtractor;
        private readonly IStaffsExtractor _staffsExtractor;
        private readonly IStaffSchoolAssociationExtractor _staffSchoolAssociationExtractor;
        private readonly IStaffSectionAssociationExtractor _staffSectionAssociationExtractor;
        private readonly IStudentsExtractor _studentsExtractor;
        private readonly IStudentAttendanceExtractor _attendanceExtractor;
        private readonly IStudentSchoolAssociationExtractor _studentSchoolAssociationExtractor;
        private readonly IStudentSectionAssociationExtractor _studentSectionAssociationExtractor;
        private readonly IUserRolesExtractor _userRoles;

        public AlmaApi(IDistrictSchoolsExtractor districtSchoolsExtractor,
                       ISchoolExtractor schoolExtractor,
                       ISessionsExtractor sessionsExtractor,
                       ICalendarEventsExtractor calendarsExtractor,
                       ICoursesExtractor coursesExtractor,
                       ICourseOfferingExtractor courseOfferingExtractor,
                       ISectionsExtractor sectionsExtractor,
                       IStaffsExtractor staffsExtractor,
                       IStaffSchoolAssociationExtractor staffSchoolAssociationExtractor,
                       IStaffSectionAssociationExtractor staffSectionAssociationExtractor,
                       IStudentsExtractor studentsExtractor,
                       IStudentAttendanceExtractor attendanceExtractor,
                       IStudentSchoolAssociationExtractor studentSchoolAssociationExtractor,
                       IStudentSectionAssociationExtractor studentSectionAssociationExtractor,
                       IUserRolesExtractor userRoles)
        {
            _districtSchoolsExtractor = districtSchoolsExtractor;
            _schoolExtractor = schoolExtractor;
            _sessionsExtractor = sessionsExtractor;
            _calendarsExtractor = calendarsExtractor;
            _coursesExtractor = coursesExtractor;
            _courseOfferingExtractor = courseOfferingExtractor;
            _sectionsExtractor = sectionsExtractor;
            _staffsExtractor = staffsExtractor;
            _staffSchoolAssociationExtractor = staffSchoolAssociationExtractor;
            _staffSectionAssociationExtractor = staffSectionAssociationExtractor;
            _studentsExtractor = studentsExtractor;
            _attendanceExtractor = attendanceExtractor;
            _studentSchoolAssociationExtractor = studentSchoolAssociationExtractor;
            _studentSectionAssociationExtractor = studentSectionAssociationExtractor;
            _userRoles = userRoles;
        }
        public IDistrictSchoolsExtractor DistrictSchools { get { return _districtSchoolsExtractor; } }
        public ISchoolExtractor School { get { return _schoolExtractor; } }
        public ISessionsExtractor Sessions { get { return _sessionsExtractor; } }
        public ISectionsExtractor Sections { get { return _sectionsExtractor; } }
        public ICalendarEventsExtractor SchoolCalendarEvents { get { return _calendarsExtractor; } }
        public ICoursesExtractor Courses { get { return _coursesExtractor; } }
        public ICourseOfferingExtractor CourseOffering { get { return _courseOfferingExtractor; } }
        public IStudentsExtractor Students { get { return _studentsExtractor; } }
        public IStudentSchoolAssociationExtractor StudentSchool { get { return _studentSchoolAssociationExtractor; } }
        public IStudentSectionAssociationExtractor StudentSection { get { return _studentSectionAssociationExtractor; } }
        public IStudentAttendanceExtractor Attendance { get { return _attendanceExtractor; } }
        public IStaffsExtractor Staff { get { return _staffsExtractor; } }
        public IStaffSectionAssociationExtractor StaffSection { get { return _staffSectionAssociationExtractor; } }
        public IStaffSchoolAssociationExtractor StaffSchool { get { return _staffSchoolAssociationExtractor; } }
        public IUserRolesExtractor UserRoles { get { return _userRoles; } }
    }

}
