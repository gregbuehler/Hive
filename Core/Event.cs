using System;
using System.Collections.Generic;
using System.Linq;

namespace Hive
{
    public class Event
    {
        public string Id { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public List<string> Lineage { get; set; }

        public Event()
        {
            Id = Guid.NewGuid().ToString();
            Data = new Dictionary<string, object>();
            Lineage = new List<string>();
        }

        public override string ToString()
        {
            var lineage = string.Join(',', this.Lineage.ToArray());
            var data = string.Join('\t', this.Data.Select(d => $"{d.Key}:{d.Value}"));

            return string.Join('\t', new string[]{
                $"id:{this.Id}",
                $"lineage:{lineage}",
                data
            });
        }
    }
}