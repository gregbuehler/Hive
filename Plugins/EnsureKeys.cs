using System.Collections.Generic;
using Hive.Core;

namespace Hive.Plugins
{
    public class EnsureKeys : Plugin
    {
        public override string Descripton => "Set event keys";

        public bool Overwrite { get; set; }
        public Dictionary<string, dynamic> Attributes { get; set; }

        public EnsureKeys(string Name, Configuration Config)
        : base(Name, Config)
        {
            this.Overwrite = Config.Options.GetValueOrDefault("overwrite", false);

            var attrs = Config.Options.GetValueOrDefault("attributes", null);
            this.Attributes = new Dictionary<string, dynamic>();
            foreach (var attr in attrs)
            {
                this.Attributes.Add(attr.Name, attr.Value);
            }
        }

        public override void Process(Event e)
        {
            foreach (var key in Attributes.Keys)
            {
                if (!e.Data.ContainsKey(key))
                {
                    e.Data.Add(key, Attributes[key]);
                }
                else
                {
                    if (Overwrite)
                    {
                        e.Data[key] = Attributes[key];
                    }
                }

            }

            e.Lineage.Add(Name);
            Emit(e);
        }
    }
}