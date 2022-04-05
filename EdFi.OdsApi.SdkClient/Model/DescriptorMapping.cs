using System;
using System.Collections.Generic;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Model
{
    public interface IDescriptorMapping
    {
        string Descriptor { get; set; }
        List<Mapping> Mapping { get; set; }
    }

    public class AcademicSubjectMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class AttendanceEventMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class CalendarEventMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class CalendarTypeMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class ClassroomPositionMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class EducationOrganizationCategoryMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class ElectronicMailTypeMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class EmploymentStatusMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class GradeLevelMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class InstitutionTelephoneNumberTypeMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class ProgramAssignmentMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class SexMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class StaffClassificationMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class TelephoneNumberTypeMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }

    public class TermMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class StudentIdentificationSystemMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class AddressTypeMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class StateAbbreviationMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class CourseIdentificationSystemMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    public class RaceMapping : IDescriptorMapping
    {
        public string Descriptor { get; set; }
        public List<Mapping> Mapping { get; set; }
    }
    
    public class Mapping
    {
        public string Src { get; set; }
        public string Dest { get; set; }
    }
}
