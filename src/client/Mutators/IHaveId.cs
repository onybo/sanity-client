using Newtonsoft.Json;

namespace Olav.Sanity.Client.Mutators
{
    public interface IHaveId
    {
        [JsonProperty("_id")]
        string Id {get;}        
    }
}