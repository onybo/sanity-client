using Newtonsoft.Json;

namespace Olav.Sanity.Client
{
    public interface ISanityDoc
    {
        [JsonProperty(PropertyName="_id")]
        string Id { get; set; }
    }
}
