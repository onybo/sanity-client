using System.Text.Json;

namespace Olav.Sanity.Client
{
    public static class JsonOptions
    {
        public static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };
    }
}
