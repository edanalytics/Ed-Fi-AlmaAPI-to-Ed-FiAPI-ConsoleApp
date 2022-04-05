using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffEducationOrganizationEmploymentAssociationTransformer
    {
        EdFiStaffEducationOrganizationEmploymentAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff);
    }
    public class StaffEducationOrganizationEmploymentAssociationTransformer : IStaffEducationOrganizationEmploymentAssociationTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StaffEducationOrganizationEmploymentAssociationTransformer(
            IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }

        public EdFiStaffEducationOrganizationEmploymentAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff)
        {
            var organizationReference = new EdFiEducationOrganizationReference(schoolId);
            var stafftReference = new EdFiStaffReference(srcStaff.id);
            return new EdFiStaffEducationOrganizationEmploymentAssociation(null,
                GetEdFiEmployeeStatusDescriptors(srcStaff.status),Convert.ToDateTime(srcStaff.created), null, organizationReference, stafftReference);

        }
        public string GetEdFiEmployeeStatusDescriptors(string srcEmploeStatus)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("EmploymentStatusDescriptor", srcEmploeStatus);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("EmploymentStatusDescriptor", "default");
            return edfiStringDescriptors;
        }
    }
}