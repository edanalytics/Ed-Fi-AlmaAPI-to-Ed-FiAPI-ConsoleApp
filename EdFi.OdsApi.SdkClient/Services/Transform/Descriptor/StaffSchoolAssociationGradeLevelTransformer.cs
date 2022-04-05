using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IStaffSchoolAssociationGradeLevelTransformer
    {
        List<EdFiStaffSchoolAssociationGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels);
    }
    public class StaffSchoolAssociationGradeLevelTransformer : IStaffSchoolAssociationGradeLevelTransformer
    {
        private readonly IDescriptorMapping _gradeLevel;
        public StaffSchoolAssociationGradeLevelTransformer(IOptions<GradeLevelMapping> mapping)
        {
            _gradeLevel = mapping.Value;
        }
        public List<EdFiStaffSchoolAssociationGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels)
        {
            var edfiGradeLevels = new List<EdFiStaffSchoolAssociationGradeLevel>();
            var almaGradeLevels = srcGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key);//Get alma Grade Levels
            foreach (var agl in almaGradeLevels)
            {
                var map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == agl);
                if (map == null)
                    map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == "default");
                edfiGradeLevels.Add(new EdFiStaffSchoolAssociationGradeLevel(map.Dest));
            }
            return edfiGradeLevels;
        }
    }
}