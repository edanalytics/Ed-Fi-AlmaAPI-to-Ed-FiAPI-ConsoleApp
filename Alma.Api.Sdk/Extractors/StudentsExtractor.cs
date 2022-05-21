using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Alma.Api.Sdk.Extractors
{
    public interface IStudentsExtractor
    {
        
        StudentsResponse Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StudentsExtractor : IStudentsExtractor
    {
        private readonly RestClient _client;

        public StudentsExtractor(IAlmaRestClientConfigurationProvider client)
        {
            _client = client.GetRestClient();
        }

        public StudentsResponse Extract(string almaSchoolCode, string schoolYearId = "")
        {
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{almaSchoolCode}/students", DataFormat.Json);
            //Synchronous call
            var response = _client.Get(request);

            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<StudentsResponse>(response);
                //Edfi needs an Integer as SchoolId, Ama api return an Alphanumeric as SchoolId,so we take StateId as SchoolId.
            var studentCount = studentResponse.response.Count;
            Console.WriteLine($"Processing {studentCount} students.");

            var studentIndex = 0;
            var stopWatch = Stopwatch.StartNew();

            Parallel.ForEach(studentResponse.response, new ParallelOptions { MaxDegreeOfParallelism=10 }, 
                student => {
                    studentIndex++;
                    if (studentIndex % 10 == 0)
                    {
                        Console.WriteLine($"    Extracting {studentIndex} students. ({stopWatch.ElapsedMilliseconds} ms - {DateTime.Now.ToLongTimeString()})");
                    }
                    student.addresses = GetStudentAddresses(almaSchoolCode, student.id);
                    student.phones = GetStudentPhones(almaSchoolCode, student.id);
                    student.emails = GetStudentEmails(almaSchoolCode, student.id);
                    student.Enrollment = GetStudentEnrollments(almaSchoolCode, student.id);
                }
            );

            stopWatch.Stop();
            Console.WriteLine($"    Done in: ({stopWatch.ElapsedMilliseconds/1000} s.)");
            return studentResponse;
        }

        private List<Address> GetStudentAddresses(string almaSchoolCode, string StudentId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/contact/addresses", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Address>();

            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<AddressessResponse>>(response);
            return studentResponse.response.addresses;
        }

        private List<Phone> GetStudentPhones(string almaSchoolCode, string StudentId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/contact/phones", DataFormat.Json);

            var response = _client.Get(request); 
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Phone>();

            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<PhonesResponse>>(response);
            return studentResponse.response.phones;
        }

        private List<Email> GetStudentEmails(string almaSchoolCode, string StudentId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/contact/email", DataFormat.Json);

            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Email>();

            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<EmailsResponse>>(response);
            return studentResponse.response.email;
        }

        private List<Enrollment> GetStudentEnrollments(string almaSchoolCode, string StudentId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/enrollment", DataFormat.Json);

            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Enrollment>();

            //Deserialize JSON data
            var responseObject = new Utf8JsonSerializer().Deserialize<EnrollmentResponse>(response);
            return responseObject.response;
        }

        private ProgramsResponse GetStudentPrograms(string almaSchoolCode, string StudentId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/students/{StudentId}/programs", DataFormat.Json);

            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new ProgramsResponse();


            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<ProgramsResponse>>(response);
            return studentResponse.response;
        }
    }
}
