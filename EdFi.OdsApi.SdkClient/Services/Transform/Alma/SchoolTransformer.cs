using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ISchoolTransformer
    {
        EdFiSchool TransformSrcToEdFi(School srcSchool);
    }

    public class SchoolTransformer : ISchoolTransformer
    {
        private readonly IDescriptorMappingService _descriptorMappingService;

        public SchoolTransformer(IDescriptorMappingService descriptorMappingService)
        {
            _descriptorMappingService = descriptorMappingService;
        }
        public EdFiSchool TransformSrcToEdFi(School srcSchool)
        {
            var edfiSchoolGradeLevelDescriptors = GetEdFiGradeLevelDescriptors(srcSchool.GradeLevels);

            return new EdFiSchool(null,
                                GetEdFiEducationOrganizationCategoryDescriptor("school"),
                                 GetEdFiGradeLevelDescriptors(srcSchool.GradeLevels),
                                Convert.ToInt32(srcSchool.stateId),null,new EdFiLocalEducationAgencyReference(Int32.Parse(srcSchool.districtId)),
                                srcSchool.addresses.Select(x => GetEdFiOrganizationAddressDescriptors(x)).ToList(),null,null,null,null,null,
                                srcSchool.phones.Select(x => GetEdFiTelephoneNumberDescriptors(x)).ToList(),null,null,null, srcSchool.name);
        }


        public EdFiEducationOrganizationInstitutionTelephone GetEdFiTelephoneNumberDescriptors(Phone srcPhone)
        {            
            return new EdFiEducationOrganizationInstitutionTelephone(_descriptorMappingService.MappAlmaToEdFiDescriptor("InstitutionTelephoneNumberTypeDescriptor","Main"), srcPhone.number);
        }
        public EdFiEducationOrganizationAddress GetEdFiOrganizationAddressDescriptors(Address srcAddress)
        {
            return new EdFiEducationOrganizationAddress(_descriptorMappingService.MappAlmaToEdFiDescriptor("AddressTypeDescriptor", "Mailing"),
                AlmaStateToEdFiStateAbbreviationDescriptor(srcAddress.state),
                srcAddress.city, srcAddress.zip, srcAddress.address, null, null, null, null, null, null, null, null, srcAddress.country, null);
        }

        private string AlmaStateToEdFiStateAbbreviationDescriptor(string almaState)
        {
            return _descriptorMappingService.MappAlmaToEdFiDescriptor("StateAbbreviationDescriptor", almaState);
        }
        public List<EdFiEducationOrganizationCategory> GetEdFiEducationOrganizationCategoryDescriptor(string AlmaOrganizationCategory)
        {
            var EducationOrganizationCategory = new List<EdFiEducationOrganizationCategory>();
            var map = _descriptorMappingService.MappAlmaToEdFiDescriptor("EducationOrganizationCategoryDescriptor", AlmaOrganizationCategory);
            if (map == null)
                map = _descriptorMappingService.MappAlmaToEdFiDescriptor("EducationOrganizationCategoryDescriptor", "default");
            EducationOrganizationCategory.Add(new EdFiEducationOrganizationCategory(map));
            return EducationOrganizationCategory;
        }

        private List<EdFiSchoolGradeLevel> GetEdFiGradeLevelDescriptors(List<GradeLevel> almaGradeLevels)
        {
            var almaGl = almaGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key).ToList();//Get alma Grade Levels

            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("GradeLevelDescriptor", almaGl);
            var edfiGradeLevels = new List<EdFiSchoolGradeLevel>();
            foreach (var agl in edfiStringDescriptors)
            {
                if (!edfiGradeLevels.Contains(new EdFiSchoolGradeLevel(agl))){
                    edfiGradeLevels.Add(new EdFiSchoolGradeLevel(agl));
                }
            }
            return edfiGradeLevels;
        }
    }
}
