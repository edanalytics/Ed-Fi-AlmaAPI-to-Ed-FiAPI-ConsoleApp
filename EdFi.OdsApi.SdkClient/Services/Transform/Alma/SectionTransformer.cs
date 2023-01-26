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
    public class SectionTransformer : ISectionTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        public SectionTransformer(ISessionNameTransformer sessionNameTransformer)
        {
            _sessionNameTransformer = sessionNameTransformer;
        }
        public List<EdFiSection> TransformSrcToEdFi(int schoolId, List<Section> almaSection, List<Session> almaSessions)
        {
            var edfiSecions = new List<EdFiSection>();
            foreach (var section in almaSection)
            {
                //The same class could exist in diferent periods
                if (section.gradingPeriods != null) //TODO: Figure out why these are null.
                {
                    foreach (var term in section.gradingPeriods)
                    {
                        var courseCode = string.IsNullOrEmpty(section.Course.code) ? section.Course.id : section.Course.code;
                        edfiSecions.Add(new EdFiSection(null, section.id,
                        new EdFiCourseOfferingReference(courseCode, schoolId, section.SchoolYear.endDate.Year,
                           _sessionNameTransformer.TransformSrcToEdFi(term, almaSessions)),
                        null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, section.name));

                    }
                }
                

            }
            return edfiSecions;
        }
    }
}
