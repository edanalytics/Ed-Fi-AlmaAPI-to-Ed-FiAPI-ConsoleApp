using Alma.Api.Sdk.Models;
using EdFi.OdsApi.Sdk.Models.Resources;
namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEducationOrganizationInstitutionTelephoneTransformer
    {
        EdFiEducationOrganizationInstitutionTelephone TransformSrcToEdFi(Phone srcPhone);
    }
    public class EducationOrganizationInstitutionTelephoneTransformer : IEducationOrganizationInstitutionTelephoneTransformer
    {
        public EdFiEducationOrganizationInstitutionTelephone TransformSrcToEdFi(Phone srcPhone)
        {
            return new EdFiEducationOrganizationInstitutionTelephone("uri://ed-fi.org/InstitutionTelephoneNumberTypeDescriptor#Main", srcPhone.number);
        }
    }
}
