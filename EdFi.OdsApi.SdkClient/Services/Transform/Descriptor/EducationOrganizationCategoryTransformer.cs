using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEducationOrganizationCategoryTransformer
    {
        List<EdFiEducationOrganizationCategory> TransformSrcToEdFi(string AlmaOrganizationCategory);
    }
    public class EducationOrganizationCategoryTransformer : IEducationOrganizationCategoryTransformer
    {
        private readonly IDescriptorMapping _organizationCategory;
        public EducationOrganizationCategoryTransformer(IOptions<EducationOrganizationCategoryMapping> mapping)
        {
            _organizationCategory = mapping.Value;
        }
        public List<EdFiEducationOrganizationCategory> TransformSrcToEdFi(string AlmaOrganizationCategory)
        {
            var EducationOrganizationCategory = new List<EdFiEducationOrganizationCategory>();
            var map = _organizationCategory.Mapping.SingleOrDefault(x => x.Src == AlmaOrganizationCategory);
            if (map == null)
                map = _organizationCategory.Mapping.SingleOrDefault(x => x.Src == "default");
            EducationOrganizationCategory.Add(new EdFiEducationOrganizationCategory(map.Dest));
            return EducationOrganizationCategory;
        }
    }
}
