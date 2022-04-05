using Alma.Api.Sdk.Models;
using EdFi.OdsApi.Sdk.Models.Resources;
using System.Collections.Generic;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Alma
{
    public interface IStudentSectionAssociationTransformer
    {
        List<EdFiStudentSectionAssociation> TransformSrcToEdFi(int schoolId, StudentSectionResponse srcSectionAssociation, List<Session> almaSessions);
    }
    public class StudentSectionAssociationTransformer : IStudentSectionAssociationTransformer
    {
        private readonly ISessionNameTransformer _sessionNameTransformer;
        public StudentSectionAssociationTransformer(ISessionNameTransformer sessionNameTransformer)
        {
            _sessionNameTransformer = sessionNameTransformer;
        }
        public List<EdFiStudentSectionAssociation> TransformSrcToEdFi(int schoolId, StudentSectionResponse srcSectionAssociation, List<Session> almaSessions)
        {
            var sectionAssociationList = new List<EdFiStudentSectionAssociation>();
            foreach (var classes in srcSectionAssociation.classes)
            {
                if (classes.Course != null)
                {
                    var courseCode = string.IsNullOrEmpty(classes.Course.code) ? classes.Course.id : classes.Course.code;
                    var sessionName = _sessionNameTransformer.TransformSrcToEdFi(almaSessions, classes.Course.schoolYearId, classes.Course.effectiveDate);
                    var sectionReference = new EdFiSectionReference(courseCode, schoolId, classes.SchoolYear.endDate.Year, classes.id, sessionName, null);
                    var studentReference = new EdFiStudentReference(srcSectionAssociation.StudentId);
                    sectionAssociationList.Add(new EdFiStudentSectionAssociation(null, classes.SchoolYear.startDate, sectionReference, studentReference));
                }
            }
            return sectionAssociationList;
        }
    }
}
