using Alma.Api.Sdk.Extractors;
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffSchoolAssociationTransformer
    {
        EdFiStaffSchoolAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff, string almaSchoolCode, List<UserRole> userRoles);
    }
    public class StaffSchoolAssociationTransformer : IStaffSchoolAssociationTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StaffSchoolAssociationTransformer(
            IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }

       public EdFiStaffSchoolAssociation TransformSrcToEdFi(int schoolId, Staff srcStaff,string almaSchoolCode, List<UserRole> userRoles)
        {
            var Rol = userRoles.Where(r => r.id == srcStaff.roleId).FirstOrDefault().name;
            var schoolReference = new EdFiSchoolReference(schoolId);
            var stafftReference = new EdFiStaffReference(srcStaff.id);
            return new EdFiStaffSchoolAssociation(null, GetEdFiProgramAssignemtDescriptors(Rol), null, schoolReference, null, stafftReference);

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