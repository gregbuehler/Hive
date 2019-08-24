using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hive.Core;
using Newtonsoft.Json.Linq;

namespace Hive.Plugins
{
    public class Explode : Plugin
    {
        public override string Descripton => "Explode facets";

        public string Key { get; set; }
        public string Mode { get; set; }

        public string Delimiter { get; set; }
        public string Seperator { get; set; }

        public Explode(string Name, Configuration Config)
        : base(Name, Config)
        {
            this.Key = Config.Options.GetValueOrDefault("key", "_raw");
            this.Mode = Config.Options.GetValueOrDefault("mode", "delimited");
            this.Delimiter = Config.Options.GetValueOrDefault("delimiter", "\t");
            this.Seperator = Config.Options.GetValueOrDefault("seperator", ":");
        }

        private Event ExplodeJson(Event e, string key)
        {
            var value = e.Data.GetValueOrDefault(key, "{}").ToString();
            var j = JContainer.Parse(value);
            var v = j.ToObject<Dictionary<string, dynamic>>();

            foreach (var k in v.Keys)
            {
                e.Data.Add(k, v[k].ToString());
            }

            return e;
        }

        private Event ExplodeDelimited(Event e, string key, string delimiter="\t", string seperator=":")
        {
            var value = e.Data.GetValueOrDefault(key, "").ToString();
            foreach (var kvp in value.Split(delimiter))
            {
                var parts = kvp.Split(seperator);
                e.Data.Add(parts[0], parts[1]);
            }

            return e;
        }

        public override void Process(Event e)
        {
            e.Lineage.Add(Name);

            Console.WriteLine($"Processing {e}");

            switch (Mode)
            {
                case "json":
                    e = ExplodeJson(e, Key);
                    break;
                case "delimited":
                    e = ExplodeDelimited(e, Key, Delimiter, Seperator);
                    break;
                default:
                    throw new Exception($"Undefined mode '{Mode}'");
            }

            Emit(e);
        }
    }
}