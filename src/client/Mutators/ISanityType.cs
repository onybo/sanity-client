using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Olav.Sanity.Client.Mutators
{
    public interface ISanityType
    {
        [JsonProperty("_type")]
        string SanityTypeName { get; }
    }
}