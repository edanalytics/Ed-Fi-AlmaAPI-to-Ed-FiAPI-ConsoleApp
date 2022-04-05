using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ICourseOfferingOfferedGradeLevelTransformer
    {
        List<EdFiCourseOfferingOfferedGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels);
    }
    public class CourseOfferingOfferedGradeLevelTransformer : ICourseOfferingOfferedGradeLevelTransformer
    {
        private readonly IDescriptorMapping _gradeLevel;
        public CourseOfferingOfferedGradeLevelTransformer(IOptions<GradeLevelMapping> settings)
        {
            _gradeLevel = settings.Value;
        }
        public List<EdFiCourseOfferingOfferedGradeLevel> TransformSrcToEdFi(List<GradeLevel> srcGradeLevels)
        {
            var edfiGradeLevels = new List<EdFiCourseOfferingOfferedGradeLevel>();
            var almaGradeLevels = srcGradeLevels.GroupBy(item => item.gradeLevelAbbr).Select(glevel => glevel.Key);//Get alma Grade Levels
            foreach (var agl in almaGradeLevels)
            {
                var map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == agl);
                if (map == null)
                    map = _gradeLevel.Mapping.SingleOrDefault(x => x.Src == "default");
                edfiGradeLevels.Add(new EdFiCourseOfferingOfferedGradeLevel(map.Dest));
            }
            return edfiGradeLevels;
        }
    }
}

