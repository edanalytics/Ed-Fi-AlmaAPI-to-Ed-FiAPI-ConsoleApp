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
    public interface IStaffsExtractor
    {
        StaffResponse Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StaffsExtractor : IStaffsExtractor
    {
        private readonly RestClient _client;
        public StaffsExtractor(IAlmaRestClientConfigurationProvider client,
                               ISectionsExtractor sectionsExtractor)
        {
            _client = client.GetRestClient();
        }
        public StaffResponse Extract(string almaSchoolCode, string schoolYearId = "")
        {
            //Request generation (set resource and response data format)
            var request = new RestRequest($"v2/{almaSchoolCode}/staff", DataFormat.Json);
            //Synchronous call
            var response = _client.Get(request);

            //Deserialize JSON data
            var staffResponse = new Utf8JsonSerializer().Deserialize<StaffResponse>(response);
            var recordIndex = 0;
            var stopWatch = Stopwatch.StartNew();
            Parallel.ForEach(staffResponse.response, new ParallelOptions { MaxDegreeOfParallelism = 20 },
               staff => {
                   staff.addresses = GetStaffAddresses(almaSchoolCode, staff.id);
                   staff.phones = GetStaffPhones(almaSchoolCode, staff.id);
                   staff.emails = GetStaffEmails(almaSchoolCode, staff.id);
                   recordIndex++;
                   if (recordIndex % 10 == 0)
                   {
                       Console.WriteLine($"    Extracting {recordIndex} Staffs. ({stopWatch.ElapsedMilliseconds} ms - {DateTime.Now.ToLongTimeString()})");
                   }
               }
           );
            stopWatch.Stop();
            Console.WriteLine($"    Done in: ({stopWatch.ElapsedMilliseconds / 1000} s.)");
            return staffResponse;

        }

        private List<Address> GetStaffAddresses(string almaSchoolCode, string StaffId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/staff/{StaffId}/contact/addresses", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Address>();
            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<AddressessResponse>>(response);
            return studentResponse.response.addresses;
        }
        private List<Phone> GetStaffPhones(string almaSchoolCode, string StaffId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/staff/{StaffId}/contact/phones", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Phone>();
            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<PhonesResponse>>(response);
            return studentResponse.response.phones;
        }

        private List<Email> GetStaffEmails(string almaSchoolCode, string StaffId)
        {
            var request = new RestRequest($"v2/{almaSchoolCode}/staff/{StaffId}/contact/email", DataFormat.Json);
            var response = _client.Get(request);
            if (response.StatusCode != HttpStatusCode.OK)
                return new List<Email>();
            //Deserialize JSON data
            var studentResponse = new Utf8JsonSerializer().Deserialize<Response<EmailsResponse>>(response);
            return studentResponse.response.email;
        }
    }
}

