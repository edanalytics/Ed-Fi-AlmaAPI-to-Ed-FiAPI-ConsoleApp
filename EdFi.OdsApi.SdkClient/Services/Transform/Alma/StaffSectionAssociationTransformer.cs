using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
using EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor;
using EdFi.OdsApi.Sdk.Models.Resources;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffSectionAssociationTransformer
    {
        List<EdFiStaffSectionAssociation> TransformSrcToEdFi(int schoolId, StaffSection srcSectionAssociation, List<Session> almaSessions, List<UserRole> userRoles);
    }
    public class StaffSectionAssociationTransformer : IStaffSectionAssociationTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        private readonly ILogger<StaffSectionAssociationTransformer> _logger;
        private readonly IDescriptorMappingService _descriptorMappingService;
        public StaffSectionAssociationTransformer(IDescriptorMappingService descriptorMappingService,
            ILogger<StaffSectionAssociationTransformer> logger, ISessionNameTransformer sessionNameTransformer)
        {
            _descriptorMappingService = descriptorMappingService;
            _sessionNameTransformer = sessionNameTransformer;
            _logger = logger;
        }
        public List<EdFiStaffSectionAssociation> TransformSrcToEdFi(int schoolId, StaffSection srcSectionAssociation, List<Session> almaSessions, List<UserRole> userRoles)
        {
            var sectionAssociationList = new List<EdFiStaffSectionAssociation>();
            foreach (var classes in srcSectionAssociation.classes)
            {
                if (classes.Course != null)
                {
                    if (string.IsNullOrEmpty(classes.endDate))
                    {
                        _logger.LogWarning($"The class with id {classes.id} is missing startDate or endDate   /staff/{srcSectionAssociation.StaffId}/ classes");
                    }
                    else
                    {
                        foreach (var term in classes.gradingPeriods)
                        {
                            var Rol = userRoles.Where(r => r.id == srcSectionAssociation.roleId).FirstOrDefault().name;
                            var courseCode = string.IsNullOrEmpty(classes.Course.code) ? classes.Course.id : classes.Course.code;
                            //Get the correct Session name
                            var sessionName = _sessionNameTransformer.TransformSrcToEdFi(term, almaSessions);
                            var sectionReference = new EdFiSectionReference(courseCode, schoolId, Convert.ToDateTime(classes.Course.SchoolYear.endDate).Year, classes.id, sessionName, null);
                            var staffReference = new EdFiStaffReference(srcSectionAssociation.StaffId);
                            sectionAssociationList.Add(new EdFiStaffSectionAssociation(null, sectionReference, staffReference, Convert.ToDateTime(classes.startDate),
                                GetEdfiClassroomPositionDescriptors(Rol)));
                        }



                    }
                }

            }
            return sectionAssociationList;
        }

        private string GetEdfiClassroomPositionDescriptors(string srcClassroomPosition)
        {
            var edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ClassroomPositionDescriptor", srcClassroomPosition);
            if (edfiStringDescriptors == null)
                edfiStringDescriptors = _descriptorMappingService.MappAlmaToEdFiDescriptor("ClassroomPositionDescriptor", "Teacher");
            return edfiStringDescriptors;
        }
    }
}
