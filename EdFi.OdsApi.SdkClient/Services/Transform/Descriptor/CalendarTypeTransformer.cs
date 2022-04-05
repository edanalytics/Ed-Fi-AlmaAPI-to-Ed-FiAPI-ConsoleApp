using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface ICalendarTypeTransformer
    {
        string TransformSrcToEdFi(string srcCalendarTypeDescriptor);
    }
    public class CalendarTypeTransformer : ICalendarTypeTransformer
    {
        private readonly IDescriptorMapping _calendarType;
        public CalendarTypeTransformer(IOptions<CalendarTypeMapping> mapping)
        {
            _calendarType = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcCalendarTypeDescriptor)
        {
            var map = _calendarType.Mapping.SingleOrDefault(x => x.Src == srcCalendarTypeDescriptor);
            if (map == null)
                map = _calendarType.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
