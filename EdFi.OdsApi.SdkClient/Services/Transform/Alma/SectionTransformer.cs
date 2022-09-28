using Alma.Api.Sdk.Models;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface ISectionTransformer
    {
        List<EdFiSection> TransformSrcToEdFi(int schoolId, List<Section> almaSection, List<Session> almaSessions);
    }
    public class SectionTransformer: ISectionTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        public SectionTransformer(ISessionNameTransformer sessionNameTransformer)
        {
            _sessionNameTransformer = sessionNameTransformer;
        }
        public List<EdFiSection> TransformSrcToEdFi(int schoolId, List<Section> almaSection, List<Session> almaSessions)
        {
            return almaSection.Select(aSec =>
                new EdFiSection(null, aSec.id,
                    new EdFiCourseOfferingReference(string.IsNullOrEmpty(aSec.Course.code) ? aSec.Course.id : aSec.Course.code, schoolId, aSec.SchoolYear.endDate.Year, 
                        _sessionNameTransformer.TransformSrcToEdFi(almaSessions, aSec.schoolYearId, aSec.Course.effectiveDate)),
                    null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, aSec.name)
            ).ToList();
        }
    }
}
