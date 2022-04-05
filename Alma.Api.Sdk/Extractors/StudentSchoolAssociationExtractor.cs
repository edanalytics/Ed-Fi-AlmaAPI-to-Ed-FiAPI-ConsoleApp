using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alma.Api.Sdk.Extractors
{
    public interface IStudentSchoolAssociationExtractor
    {
        List<Enrollment> Extract(string almaSchoolCode);
    }

    public class StudentSchoolAssociationExtractor : IStudentSchoolAssociationExtractor
    {
        private readonly RestClient _client;
        private readonly IStudentsEnrollmentsExtractor _studentsEnrollmentsExtractor;
        private readonly IStudentsExtractor _studentsExtractor;
        public StudentSchoolAssociationExtractor(IAlmaRestClientConfigurationProvider client,
            IStudentsExtractor studentsExtractor,
            IStudentsEnrollmentsExtractor studentsEnrollmentsExtractor)
        {
            _client = client.GetRestClient();
            _studentsEnrollmentsExtractor = studentsEnrollmentsExtractor;
            _studentsExtractor = studentsExtractor;
        }
        public List<Enrollment> Extract(string almaSchoolCode)
        {
            var studentEnrollments = _studentsEnrollmentsExtractor.Extract(almaSchoolCode);

            var almaStudents = _studentsExtractor.Extract(almaSchoolCode);
            var studentsSchoolAssociation = new List<Enrollment>();
            var count = almaStudents.response.Count;
            Console.WriteLine($"Processing Enrollments from  {count} students.");
            almaStudents.response.ForEach(stud =>
            {
                studentEnrollments.students.FirstOrDefault(student => student.id == stud.id).Enrollment.ForEach(enroll =>
                {
                    enroll.studentId = stud.id;
                    studentsSchoolAssociation.Add(enroll);
                });
            });
            return studentsSchoolAssociation;
        }
    }
}