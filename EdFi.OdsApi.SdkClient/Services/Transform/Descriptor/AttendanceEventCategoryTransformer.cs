using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IAttendanceEventCategoryTransformer
    {
        string TransformSrcToEdFi(string srcreportedStatus, string srcStatusModifier);
    }
    public class AttendanceEventCategoryTransformer : IAttendanceEventCategoryTransformer
    {
        private readonly IDescriptorMapping _attendanceEvent;
        public AttendanceEventCategoryTransformer(IOptions<AttendanceEventMapping> mapping)
        {
            _attendanceEvent = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcreportedStatus, string srcStatusModifier)
        {
            if (srcreportedStatus == "Absent")
                if (srcStatusModifier == "Excused" || srcStatusModifier == "Unexcused")
                    srcreportedStatus = $"{srcStatusModifier} {srcreportedStatus}";
            var map = _attendanceEvent.Mapping.SingleOrDefault(x => x.Src == $"{srcreportedStatus}".Trim());
            if (map == null)
                map = _attendanceEvent.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
