using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace sanity
{
    public class NdJsonConvert
    {
        public static List<T> Deserialize<T>(string ndJson)
        {
            var jsonSegments = ndJson.Split(Environment.NewLine.ToCharArray());
            return jsonSegments.Where(json => !string.IsNullOrWhiteSpace(json))
                .Select(JsonConvert.DeserializeObject<T>)
                .ToList();
        }
    }
}
