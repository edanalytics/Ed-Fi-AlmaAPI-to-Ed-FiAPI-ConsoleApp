using EdFi.AlmaToEdFi.Cmd.Model;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.AlmaToEdFi.Cmd.Services.Transform.Descriptor
{
    public interface IStateAbbreviationTransformer
    {
        string TransformSrcToEdFi(string srcState);
    }
    public class StateAbbreviationTransformer : IStateAbbreviationTransformer
    {
        private readonly IDescriptorMapping _stateAbbr;
        public StateAbbreviationTransformer(IOptions<StateAbbreviationMapping> mapping)
        {
            _stateAbbr = mapping.Value;
        }
        public string TransformSrcToEdFi(string srcState)
        {
            var map = _stateAbbr.Mapping.SingleOrDefault(x => x.Src == srcState);
            if (map == null)
                map = _stateAbbr.Mapping.SingleOrDefault(x => x.Src == "default");
            return map.Dest;
        }
    }
}
