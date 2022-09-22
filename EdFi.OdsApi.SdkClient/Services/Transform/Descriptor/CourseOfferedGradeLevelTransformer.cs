using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ICourseOfferedGradeLevelTransformer
    {
        List<EdFiCourseOfferedGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels);
    }
    public class CourseOfferedGradeLevelTransformer : ICourseOfferedGradeLevelTransformer
    {
        private readonly IDescriptorMapping _gradeLevel;
        public CourseOfferedGradeLevelTransformer(IOptions<GradeLevelMapping> mapping)
        {
            _gradeLevel = mapping.Value;
        }
        public List<EdFiCourseOfferedGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels)
        {
            var edfiGradeLevels = new List<EdFiCourseOfferedGradeLevel>();
            var almaGradeLevels = srcGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key);//Get alma Grade Levels
            foreach (var agl in almaGradeLevels)
            {
                var map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == agl);
                if (map == null)
                    map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == "default");
                if (!edfiGradeLevels.Contains(new EdFiCourseOfferedGradeLevel(map.Dest)))
                {
                    edfiGradeLevels.Add(new EdFiCourseOfferedGradeLevel(map.Dest));
                }
                
            }
            return edfiGradeLevels;
        }
    }
}
