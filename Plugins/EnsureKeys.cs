using System.Collections.Generic;
using Hive.Core;

namespace Hive.Plugins
{
    public class EnsureKeys : Plugin
    {
        public override string Descripton => "Set event keys";

        public Dictionary<string, object> Foo { get; set; }

        public EnsureKeys(string Name, System.Collections.Generic.Dictionary<string, dynamic> Configuration)
        : base(Name, Configuration)
        {
            this.Foo = Configuration.GetValueOrDefault("Foo", null);
        }

        public override void Process(Event e)
        {
            foreach (var key in Foo.Keys)
            {
                if (!e.Data.ContainsKey(key))
                {
                    e.Data.Add(key, Foo[key]);
                }
            }

            e.Lineage.Add(Name);
            Emit(e);
        }
    }
}