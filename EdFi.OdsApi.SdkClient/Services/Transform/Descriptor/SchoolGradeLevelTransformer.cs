using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ISchoolGradeLevelTransformer
    {
        List<EdFiSchoolGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels);
    }
    public class SchoolGradeLevelTransformer : ISchoolGradeLevelTransformer
    {
        private readonly IDescriptorMapping _gradeLevel;
        public SchoolGradeLevelTransformer(IOptions<GradeLevelMapping> mapping)
        {
            _gradeLevel = mapping.Value;
        }

        public List<EdFiSchoolGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels)
        {
            var edfiGradeLevels = new List<EdFiSchoolGradeLevel>();
            var almaGradeLevels = srcGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key);//Get alma Grade Levels
            foreach (var agl in almaGradeLevels)
            {
                var map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == agl);
                 if(map==null)
                     map = _gradeLevel.Mapping.SingleOrDefault(x=> x.Src=="default");
                edfiGradeLevels.Add(new EdFiSchoolGradeLevel(map.Dest));
            }
            return edfiGradeLevels;
        }
    }
}