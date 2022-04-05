using System.Collections.Generic;

namespace Alma.Api.Sdk.Models
{
    public class Response<T>
    {
        public T response { get; set; }
        public List<Link> _links { get; set; }
    }

    public class Link
    {
        public string rel { get; set; }
        public string href { get; set; }
    }
}
