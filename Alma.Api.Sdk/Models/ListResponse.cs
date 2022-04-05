using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class ListResponse<T>
    {
        public List<T> response { get; set; }
        public List<Link> _links { get; set; }
    }
}
