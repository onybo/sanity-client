using System.Text.Json.Serialization;

namespace Olav.Sanity.Client.Mutators
{
    public interface IHaveId
    {
        [JsonPropertyName("_id")]
        string Id {get;}
    }
}
