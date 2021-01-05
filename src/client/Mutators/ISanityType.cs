using System.Text.Json.Serialization;

namespace Olav.Sanity.Client.Mutators
{
    public interface ISanityType
    {
        [JsonPropertyName("_type")]
        string SanityTypeName { get; }
    }
}
