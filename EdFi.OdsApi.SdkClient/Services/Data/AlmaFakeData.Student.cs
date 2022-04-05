using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Data
{
    public static class AlmaFakeData
    {
        //School data
        public static string SchoolName = "Guemez Highschool";
        public static Int32 SchoolId = 111;
        public static Int32 DistrictId = 255901;
        public static string EducationOrganizationCategoryDescriptor = "School";
        public static List<string> GradeLevels = new List<string> { "1st", "2nd" };

        //Session data
        public static string SessionName = "2021-2022 Spring Semester Session";
        public static Int32 SchoolYear = 2025;
        public static DateTime BeginDate = DateTime.Now.AddDays(-20);
        public static DateTime EndDate = DateTime.Now.AddDays(17);
        public static string TermDescriptor = "semester";
        public static Int16 TotalInstructionalDays =10;

        //Calendar data
        public static string Calendarcode = "GuemezHighCalendar01";
        public static string CalendarTypeDescriptor = "uri://ed-fi.org/CalendarTypeDescriptor#School";
        public static List<string> Events = new List<string> { "Instructional" };

        //course data
        public static string CourseCode = "GuemezAlBar";
        public static string IdentificationCode = "GALG-1";
        public static string CourseTitle = "Guemez Algebra";
        public static List<string> CourseIdentificationSystem = new List<string> { "state" };
        public static Int16 NumberOfParts = 1;

        //course offering Data
        public static string CourseOfferingName = "Guemez Alg Offering";
        public static string LocalCourseCode = "GuemezAlBar-1";

        //Section data
        public static string SectionName = "2021-2022 Spring Semester Section";
        public static string SectionIdentifier = "SpringGuemezAlg1SectionId";

        //Student 1 data
        public static string StudentId1 = "Stu001";
        public static string StudentLastName1 = "John";
        public static string StudentFirstName1 = "DePerez";
        public static string StudentGender1 = "M";

        //Student 2 data
        public static string StudentId2 = "Stu002";
        public static string StudentLastName2 = "Jane";
        public static string StudentFirstName2 = "DePerez";
        public static string StudentGender2 = "F";


        //Student School Association Data
        public static string EntryGradeLevelDescriptor = "first";

        //Student Education Organization Association Data
        public static List<string> CourseIdentificationSystemDescriptor = new List<string> { "state" };
        //public static Int32 EducationOrganizationReference = 255901001;
        public static string  Email = "angel@angel.com";
        public static string Sex = "male";

        //Student Section Association Data
        public static DateTime BeginDateSectionAssociation = DateTime.Now.AddDays(-34);

        //Student School Attendance Event
        public static DateTime EventDate = DateTime.Now;
        public static string AttendanceEventCategoryDescriptor = "absense";
    }
}
