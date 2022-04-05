using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IEmploymentStatusTransformer
    {
        string TransformSrcToEdFi(string srcEmployeeStatus);
    }
    public class EmploymentStatusTransformer : IEmploymentStatusTransformer
    {
        private readonly IDescriptorMapping _employeeStatus;
        public EmploymentStatusTransformer(IOptions<EmploymentStatusMapping> mapping)
        {
            _employeeStatus = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcEmployeeStatus)
        {
            var map = _employeeStatus.Mapping.SingleOrDefault(x => x.Src == srcEmployeeStatus);
            if (map == null)
                map = _employeeStatus.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
