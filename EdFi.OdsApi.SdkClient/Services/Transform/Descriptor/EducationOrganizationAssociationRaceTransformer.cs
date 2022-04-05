using EdFi.AlmaToEdFi.Cmd.Model;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEducationOrganizationAssociationRaceTransformer
    {
        List<EdFiStudentEducationOrganizationAssociationRace> TransformSrcToEdFi(List<string> srcRace);
    }
    public class EducationOrganizationAssociationRaceTransformer : IEducationOrganizationAssociationRaceTransformer
    {
        private readonly IDescriptorMapping _race;
        public EducationOrganizationAssociationRaceTransformer(IOptions<RaceMapping> mapping)
        {
            _race = mapping.Value;
        }
        public List<EdFiStudentEducationOrganizationAssociationRace> TransformSrcToEdFi(List<string> srcRace)
        {
            var StudentEducationOrganizationAssociationRace = new List<EdFiStudentEducationOrganizationAssociationRace>();
            foreach (var race in srcRace)
            {
                var map = _race.Mapping.SingleOrDefault(x => x.Src == race);
                if (map == null)
                    map = _race.Mapping.SingleOrDefault(x => x.Src == "default");
                StudentEducationOrganizationAssociationRace.Add(new EdFiStudentEducationOrganizationAssociationRace(map.Dest));
            }
            return StudentEducationOrganizationAssociationRace;
        }
    }
}
