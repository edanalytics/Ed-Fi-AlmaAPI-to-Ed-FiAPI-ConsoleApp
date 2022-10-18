using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffEducationOrganizationAssignmentTransformer
    {
        EdFiStaffEducationOrganizationAssignmentAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff);
    }
    public class StaffEducationOrganizationAssignmentTransformer : IStaffEducationOrganizationAssignmentTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StaffEducationOrganizationAssignmentTransformer(
            IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
     
        public EdFiStaffEducationOrganizationAssignmentAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff)
        {
            var organizationReference = new EdFiEducationOrganizationReference(schoolId);
            var stafftReference = new EdFiStaffReference(srcStaff.id);

            var stafftEmploymentReference = new EdFiStaffEducationOrganizationEmploymentAssociationReference(schoolId,
                GetEdFiEmployeeStatusDescriptors(srcStaff.status), srcStaff.created, srcStaff.id);

            srcStaff.Rol = Helpers.StudentTranslation.GetStaffUserRole(schoolId, srcStaff.roleId).name;

            return new EdFiStaffEducationOrganizationAssignmentAssociation(null, srcStaff.created,
                GetEdFiStaffClassiificationDescriptors(srcStaff.Rol), null, organizationReference, stafftEmploymentReference , stafftReference);

        }
        public string GetEdFiEmployeeStatusDescriptors(string srcEmploeStatus)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("EmploymentStatusDescriptor", srcEmploeStatus);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("EmploymentStatusDescriptor", "default");
            return edfiStringDescriptors;
        }
        public string GetEdFiStaffClassiificationDescriptors(string srcRol)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("StaffClassificationDescriptor", srcRol);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("StaffClassificationDescriptor", "default");
            return edfiStringDescriptors;
        }
    }
}
