using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffSchoolAssociationsGradesTransformer
    {
        List<EdFiStaffSchoolAssociation> TransformSrcToEdFi(int schoolId, StaffSection srcStaff, List<UserRole> userRoles);
    }
    public class StaffSchoolAssociationsGradesTransformer : IStaffSchoolAssociationsGradesTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StaffSchoolAssociationsGradesTransformer(
            IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public List<EdFiStaffSchoolAssociation> TransformSrcToEdFi(int schoolId, StaffSection srcStaff, List<UserRole> userRoles)
        {
            var staffSchool = new List<EdFiStaffSchoolAssociation>();
            foreach (var classes in srcStaff.classes)
            {
                if (classes.Course != null)
                {
                    var Rol = userRoles.FirstOrDefault(r => r.id == srcStaff.roleId).name;
                    var schoolReference = new EdFiSchoolReference(schoolId);
                    var schoolYearReference = new EdFiSchoolYearTypeReference(classes.Course.SchoolYear.endDate.Year);
                    var stafftReference = new EdFiStaffReference(srcStaff.StaffId);
                    staffSchool.Add(new EdFiStaffSchoolAssociation(null,
                        GetEdFiProgramAssignemtDescriptors(Rol), null, schoolReference, schoolYearReference, stafftReference, null,
                        GetEdFiGradeLevelDescriptors(classes.Course.GradeLevels)));
                }

            }
            return staffSchool;
        }

        private List<EdFiStaffSchoolAssociationGradeLevel> GetEdFiGradeLevelDescriptors(List<GradeLevel> almaGradeLevels)
        {
            var almaGl = almaGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key).ToList();//Get alma Grade Levels

            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", almaGl);
            var edfiGradeLevels = new List<EdFiStaffSchoolAssociationGradeLevel>();
            foreach (var agl in edfiStringDescriptors)
            {
                edfiGradeLevels.Add(new EdFiStaffSchoolAssociationGradeLevel(agl));
            }
            return edfiGradeLevels;
        }
        private string GetEdFiProgramAssignemtDescriptors(string scrRol)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ProgramAssignmentDescriptor", scrRol);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ProgramAssignmentDescriptor", "default");
            return edfiStringDescriptors;
        }
    }
}