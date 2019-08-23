using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hive.Core
{
    public class Configuration
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("options")]
        public Dictionary<string, dynamic> Options { get; set; }

        [JsonProperty("upstreams")]
        public List<string> Upstreams { get; set; }
    }
}