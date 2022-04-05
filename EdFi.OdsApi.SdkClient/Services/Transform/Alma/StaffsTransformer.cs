using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffsTransformer
    {
        EdFiStaff TransformSrcToEdFi(Staff srcStaff);
    }

    public class StaffsTransformer : IStaffsTransformer
    {
        private readonly ILeadingTrailingWhitespaceTransformer _removeWhitespaceTransformer;
        private readonly IDescriptorMappingService _descriptorMappingService;

        public StaffsTransformer(
            IDescriptorMappingService descriptorMappingService, 
            ILeadingTrailingWhitespaceTransformer removeWhitespaceTransformer)
        {
            _descriptorMappingService = descriptorMappingService;
            _removeWhitespaceTransformer = removeWhitespaceTransformer;
        }
        public EdFiStaff TransformSrcToEdFi(Staff srcStaff)
        {
            var staffBdate = ValidateDob(srcStaff.dob);
            var staffAddresses = new List<EdFiStaffAddress>();
            srcStaff.addresses.ForEach(address =>
            {
                staffAddresses.Add(new EdFiStaffAddress(GetEdFiAddressTypeDescriptors(address.type.FirstOrDefault()),
                               GetEdFiStateSAbbreviationDescriptors(_removeWhitespaceTransformer.TransformSrcToEdFi(address.state)), 
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.city), 
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.zip),
                               _removeWhitespaceTransformer.TransformSrcToEdFi(address.address),
                               null, null, null, null, null, null, null, null, _removeWhitespaceTransformer.TransformSrcToEdFi(address.country)));

            });
            var staffEmails = new List<EdFiStaffElectronicMail>();
            srcStaff.emails.ForEach(email => { staffEmails.Add(new EdFiStaffElectronicMail(GetEdFiElectronicMailTypeDescriptors("default"), email.emailAddress, null, null)); });
            var stf = new EdFiStaff(null, srcStaff.id, null, staffAddresses, null, staffBdate, null, null, staffEmails,
                                    srcStaff.firstName, null, null, null, null, null, null, null, null, srcStaff.lastName, null, null, srcStaff.middleName,
                                    null, null, null, null, null, null, GetEdFiGenderDescriptors(srcStaff.gender));
            return stf;
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
        public DateTime? ValidateDob(string  dob)
        {
            var staffBdate = new DateTime?();
            if (dob != null)
                if (dob == "")
                    staffBdate = null;
                else
                    staffBdate = Convert.ToDateTime(dob);
            else
                staffBdate = null;
            return staffBdate;

        }
    }
}
