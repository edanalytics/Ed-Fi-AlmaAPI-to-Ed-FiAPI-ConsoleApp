using Alma.Api.Sdk.Extractors.Alma;
using Alma.Api.Sdk.Models;
using RestSharp;
using RestSharp.Serializers.Utf8Json;
using System.Collections.Generic;


namespace Alma.Api.Sdk.Extractors
{
    public interface IStaffSchoolAssociationExtractor
    {        
        List<Staff> Extract(string almaSchoolCode, string schoolYearId = "");
    }

    public class StaffSchoolAssociationExtractor : IStaffSchoolAssociationExtractor
    {
        private readonly IStaffsExtractor _staffsExtractor;
        
        public StaffSchoolAssociationExtractor(IStaffsExtractor staffsExtractor)
        {
            _staffsExtractor = staffsExtractor;
        }

        public List<Staff> Extract(string almaSchoolCode, string schoolYearId = "")
        {      
            return _staffsExtractor.Extract(almaSchoolCode).response;
        }
    }
}