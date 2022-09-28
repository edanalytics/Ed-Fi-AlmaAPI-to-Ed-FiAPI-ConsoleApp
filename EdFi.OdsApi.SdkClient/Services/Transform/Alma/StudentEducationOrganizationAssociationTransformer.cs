
using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentEducationOrganizationAssociationTransformer
    {
        EdFiStudentEducationOrganizationAssociation TransformSrcToEdFi(int schoolId, Student srcEnrollment);
    }
    public class StudentEducationOrganizationAssociationTransformer : IStudentEducationOrganizationAssociationTransformer
    {
        private readonly ILeadingTrailingWhitespaceTransformer _removeWhitespaceTransformer;
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StudentEducationOrganizationAssociationTransformer(
            IDescriptorMappingService descriptorMappingService, 
            ILeadingTrailingWhitespaceTransformer removeWhitespaceTransformer
            )
        {
            _descriptorMappingService = descriptorMappingService;
            _removeWhitespaceTransformer = removeWhitespaceTransformer;
        }
        public EdFiStudentEducationOrganizationAssociation TransformSrcToEdFi(int schoolId, Student srcStudent)
        {
            var studentAddresses = new List<EdFiStudentEducationOrganizationAssociationAddress>();
            srcStudent.addresses.ForEach(address =>
            {
                EdFiStudentEducationOrganizationAssociationAddress eAddress = new EdFiStudentEducationOrganizationAssociationAddress(GetEdFiAddressTypeDescriptors(address.type.FirstOrDefault()),
                               GetEdFiStateSAbbreviationDescriptors(_removeWhitespaceTransformer.TransformSrcToEdFi(address.state)),
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.city),
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.zip),
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.address),
                               null, null, null, null, null, null, null, null, _removeWhitespaceTransformer.TransformSrcToEdFi(address.country));

                if (!studentAddresses.Contains(eAddress))
                {
                    studentAddresses.Add(eAddress);
                }
                

            });
            var studentPhones = new List<EdFiStudentEducationOrganizationAssociationTelephone>();
            srcStudent.phones.ForEach(Phones =>
            {
                studentPhones.Add(new EdFiStudentEducationOrganizationAssociationTelephone(GetEdFiTelephoneNumberTypeDescriptors(Phones.type), Phones.number));

            });
            var studentEmails = new List<EdFiStudentEducationOrganizationAssociationElectronicMail>();
            srcStudent.emails.ForEach(email => { studentEmails.Add(new EdFiStudentEducationOrganizationAssociationElectronicMail(GetEdFiElectronicMailTypeDescriptors("default"),
                                                                                                                                    email.emailAddress, null, null)); });
            var educationOrganizationReference = new EdFiEducationOrganizationReference(schoolId);
            //Updated to use srcStudent.stateID instead of srcStudent.id
            EdFiStudentReference studentReference = null;
            //Check to see if the returned StateId is null. If it is then try using the AlmaStudentID.
            //This will not insert the student's information but an INFO has already been output for this student during the student posts.
            if (srcStudent.stateId != null)
            {
                studentReference = new EdFiStudentReference(srcStudent.stateId);
            }
            else
            {
                studentReference = new EdFiStudentReference(srcStudent.id);
            }
            //Updated to use srcStudent.stateID instead of srcStudent.id
            var studentIdentificationSystem = GetEdFiStudentIdentificationDescriptors(srcStudent.stateId, srcStudent.districtId, srcStudent.stateId, srcStudent.schoolId);
            if (studentIdentificationSystem.Count == 0)
                studentIdentificationSystem = null;
            return new EdFiStudentEducationOrganizationAssociation(null,
                                                                    educationOrganizationReference, 
                                                                    studentReference, 
                                                                    studentAddresses, null, null, null, null, 
                                                                    studentEmails,
                                                                    GetEdFiHispanicLatino(srcStudent.ethnicity), 
                                                                    null, null,null, null, null, null, null, null, null, null, null, null, null,
                                                                    GetEdFiRaceDescriptors( srcStudent.race),
                                                                    GetEdFiGenderDescriptors(srcStudent.gender), null, studentIdentificationSystem,
                                                                    null, studentPhones,  null,null);
            

        }
        public bool GetEdFiHispanicLatino(string srcHispanicLatino)
        {
            return srcHispanicLatino == "Hispanic Or Latino" ? true : false;
        }
        public List<EdFiStudentEducationOrganizationAssociationStudentIdentificationCode> GetEdFiStudentIdentificationDescriptors(string srcIdentificationLocalCode,
                                                                                                     string srcIdentificationDistrictCode,
                                                                                                     string srcIdentificationStateCode,
                                                                                                     string srcIdentificationSchoolCode)
        {
            var edfiDescriptors = new List<EdFiStudentEducationOrganizationAssociationStudentIdentificationCode>();
            if (!string.IsNullOrEmpty(srcIdentificationLocalCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _descriptorMappingService.MappAlmaToEdFiDescriptor("StudentIdentificationSystemDescriptor", "Local"),
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationLocalCode));
            if (!string.IsNullOrEmpty(srcIdentificationStateCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _descriptorMappingService.MappAlmaToEdFiDescriptor("StudentIdentificationSystemDescriptor", "State"),
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationStateCode));
            if (!string.IsNullOrEmpty(srcIdentificationDistrictCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _descriptorMappingService.MappAlmaToEdFiDescriptor("StudentIdentificationSystemDescriptor", "District"),
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationDistrictCode));
            if (!string.IsNullOrEmpty(srcIdentificationSchoolCode))
                edfiDescriptors.Add(new EdFiStudentEducationOrganizationAssociationStudentIdentificationCode(
                    _descriptorMappingService.MappAlmaToEdFiDescriptor("StudentIdentificationSystemDescriptor", "School"),
                    string.IsNullOrEmpty(srcIdentificationSchoolCode) ? srcIdentificationLocalCode : srcIdentificationSchoolCode, srcIdentificationSchoolCode));
            return edfiDescriptors;
        }
        public List<EdFiStudentEducationOrganizationAssociationRace> GetEdFiRaceDescriptors(List<string> srcRace)
        {
            var StudentEducationOrganizationAssociationRace = new List<EdFiStudentEducationOrganizationAssociationRace>();

            foreach (var race in srcRace)
            {
                var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("RaceDescriptor", race);
                if (edfiStringDescriptors == null)
                    edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("RaceDescriptor", "default");
                StudentEducationOrganizationAssociationRace.Add(new EdFiStudentEducationOrganizationAssociationRace(edfiStringDescriptors));
            }
            return StudentEducationOrganizationAssociationRace;
        }
        private string GetEdFiTelephoneNumberTypeDescriptors(string scrAddressType)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("TelephoneNumberTypeDescriptor", scrAddressType);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("TelephoneNumberTypeDescriptor", "default");
            return edfiStringDescriptors;
        }
        private string GetEdFiAddressTypeDescriptors(string scrAddressType)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("AddressTypeDescriptor", scrAddressType);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("AddressTypeDescriptor", "default");
            return edfiStringDescriptors;
        }
        private string GetEdFiStateSAbbreviationDescriptors(string scrState)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("StateAbbreviationDescriptor", scrState);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("StateAbbreviationDescriptor", "default");
            return edfiStringDescriptors;
        }
        private string GetEdFiElectronicMailTypeDescriptors(string scrMailType)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ElectronicMailTypeDescriptor", scrMailType);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ElectronicMailTypeDescriptor", "default");
            return edfiStringDescriptors;
        }
        private string GetEdFiGenderDescriptors(string scrGender)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("SexDescriptor", scrGender);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("SexDescriptor", "default");
            return edfiStringDescriptors;
        }
    }
}
