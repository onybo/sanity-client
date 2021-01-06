using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace Olav.Sanity.Client.Transactions
{
    public class NdJsonConvert
    {
        public static List<T> Deserialize<T>(string ndJson)
        {
            var jsonSegments = ndJson.Split(Environment.NewLine.ToCharArray());
            return jsonSegments.Where(json => !string.IsNullOrWhiteSpace(json))
                .Select(json => JsonSerializer.Deserialize<T>(json, JsonOptions.DefaultJsonSerializerOptions))
                .ToList();
        }
    }
}
