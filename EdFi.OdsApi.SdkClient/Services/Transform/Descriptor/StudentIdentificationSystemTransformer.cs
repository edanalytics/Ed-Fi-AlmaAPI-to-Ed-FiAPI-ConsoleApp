using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IStudentIdentificationSystemTransformer
    {
        List<EdFiStudentEducationOrganizationAssociationStudentIdentificationCode> TransformSrcToEdFi(string srcIdentificationLocalCode,
                                                                                                      string srcIdentificationDistrictCode,
                                                                                                      string srcIdentificationStateCode,
                                                                                                      string srcIdentificationSchoolCode);
    }
    public class StudentIdentificationSystemTransformer : IStudentIdentificationSystemTransformer
    {
        private readonly IDescriptorMapping _studentIdentification;
        public StudentIdentificationSystemTransformer(IOptions<StudentIdentificationSystemMapping> mapping)
        {
            _studentIdentification = mapping.Value;
        }

        
        public List<EdFiStudentEducationOrganizationAssociationStudentIdentificationCode> TransformSrcToEdFi(string srcIdentificationLocalCode,
                                                                                                      string srcIdentificationDistrictCode,
                                                                                                      string srcIdentificationStateCode,
                                                                                                      string srcIdentificationSchoolCode)
        {
            var edfiDescriptors = new List<EdFiStudentEducationOrganizationAssociationStudentIdentificationCode>();
            if(!string.IsNullOrEmpty(srcIdentificationLocalCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _studentIdentification.Mapping.SingleOrDefault(x => x.Src == "Local").Dest, 
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationLocalCode));
            if (!string.IsNullOrEmpty(srcIdentificationStateCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _studentIdentification.Mapping.SingleOrDefault(x => x.Src == "State").Dest,
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationStateCode));
            if (!string.IsNullOrEmpty(srcIdentificationDistrictCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _studentIdentification.Mapping.SingleOrDefault(x => x.Src == "District").Dest,
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationDistrictCode));
            if (!string.IsNullOrEmpty(srcIdentificationSchoolCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _studentIdentification.Mapping.SingleOrDefault(x => x.Src == "School").Dest,
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationSchoolCode));
            return edfiDescriptors;
        }
    }
}
