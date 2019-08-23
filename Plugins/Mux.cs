using Hive.Core;

namespace Hive.Plugins
{
    public class Mux : Plugin
    {
        public override string Descripton => "Combine multiple subscribers";

        public Mux(string Name, System.Collections.Generic.Dictionary<string, dynamic> Configuration)
        : base(Name, Configuration)
        {
        }

        public override void Process(Event e)
        {
            e.Lineage.Add(Name);
            Emit(e);
        }
    }
}