using Newtonsoft.Json;

namespace Olav.Sanity.Client
{
    public class SanityDoc
    {
        [JsonProperty(PropertyName="_id")]
        public string Id { get; set; }
    }
}
