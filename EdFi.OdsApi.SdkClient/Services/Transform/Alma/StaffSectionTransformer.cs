using Alma.Api.Sdk.Models;
using EdFi.OdsApi.Sdk.Models.Resources;
using System;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStaffSectionTransformer
    {
        List<EdFiSection> TransformSrcToEdFi(int schoolId, StaffSection srcSection, List<Session> almaSessions);
    }
    public class StaffSectionTransformer : IStaffSectionTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        public StaffSectionTransformer(ISessionNameTransformer sessionNameTransformer)
        {
            _sessionNameTransformer = sessionNameTransformer;
        }
        public List<EdFiSection> TransformSrcToEdFi(int schoolId, StaffSection srcSection, List<Session> almaSessions)
        {
            var sectionList = new List<EdFiSection>();
            foreach (var classItem in srcSection.classes)
            {
                if (classItem.Course != null)
                {
                    var courseCode = string.IsNullOrEmpty(classItem.Course.code) ? classItem.Course.id : classItem.Course.code;
                    var sessionName = _sessionNameTransformer.TransformSrcToEdFi(almaSessions, classItem.Course.schoolYearId, classItem.Course.effectiveDate);
                    var edFiCourseOfferingReference = new EdFiCourseOfferingReference(courseCode, schoolId, Convert.ToDateTime(classItem.Course.SchoolYear.endDate).Year, sessionName);
                    sectionList.Add(new EdFiSection(null, courseCode, edFiCourseOfferingReference, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, classItem.ClassName));

                }
            }
            return sectionList;
        }
    }
}
