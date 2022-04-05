using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEducationOrganizationAddressTransformer
    {
        EdFiEducationOrganizationAddress TransformSrcToEdFi(Address srcAddress);
    }

    public class EducationOrganizationAddressTransformer : IEducationOrganizationAddressTransformer
    {
        private readonly IAddressTypeTransformer _addressTypeTransformer;
        private readonly IStateAbbreviationTransformer _stateAbbreviationTransformer;
        public EducationOrganizationAddressTransformer(IAddressTypeTransformer addressTypeTransformer,
                                                       IStateAbbreviationTransformer stateAbbreviationTransformer)
        {
            _addressTypeTransformer = addressTypeTransformer;
            _stateAbbreviationTransformer = stateAbbreviationTransformer;
        }

      
        public EdFiEducationOrganizationAddress TransformSrcToEdFi(Address srcAddress)
        {
            return new EdFiEducationOrganizationAddress(_addressTypeTransformer.TransformSrcToEdFi("Mailing"),
                AlmaStateToEdFiStateAbbreviationDescriptor(srcAddress.state), 
                srcAddress.city, srcAddress.zip, srcAddress.address, null, null, null, null, null, null, null, null, srcAddress.country, null);
        }

        private string AlmaStateToEdFiStateAbbreviationDescriptor(string almaState)
        {
            return _stateAbbreviationTransformer.TransformSrcToEdFi(almaState);
        }
    }
}
