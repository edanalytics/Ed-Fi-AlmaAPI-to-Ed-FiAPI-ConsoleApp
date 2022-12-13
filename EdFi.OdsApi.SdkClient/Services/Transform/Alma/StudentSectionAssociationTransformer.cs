using Alma.Api.Sdk.Models;
using EdFi.AlmaToEdFi.Cmd.Helpers;
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
                foreach (var term in classes.gradingPeriods)
                {
                    if (classes.Course != null)
                    {
                        var courseCode = string.IsNullOrEmpty(classes.Course.code) ? classes.Course.id : classes.Course.code;
                        //Get the correct Session name
                        var sessionName = _sessionNameTransformer.TransformSrcToEdFi(term, almaSessions);
                        var sectionReference = new EdFiSectionReference(courseCode, schoolId, classes.SchoolYear.endDate.Year, classes.id, sessionName, null);

                        //Use a helper function to translate the almaID to a StateId.
                        StudentTranslation st = new StudentTranslation();
                        Student studentResponse = StudentTranslation.GetStudentById(srcSectionAssociation.StudentId);
                        EdFiStudentReference studentReference = null;
                        //Check to see if the returned StateId is null. If it is then try using the AlmaStudentID.
                        //This will not insert the student's information but an INFO has already been output for this student during the student posts.
                        if (studentResponse.stateId != null)
                        {
                            studentReference = new EdFiStudentReference(studentResponse.stateId);
                        }
                        else
                        {
                            studentReference = new EdFiStudentReference(srcSectionAssociation.StudentId);
                        }

                        sectionAssociationList.Add(new EdFiStudentSectionAssociation(null, classes.SchoolYear.startDate, sectionReference, studentReference));
                    }

                }

            }
            return sectionAssociationList;
        }
    }
}
