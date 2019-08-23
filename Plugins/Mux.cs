using Hive.Core;

namespace Hive.Plugins
{
    public class Mux : Plugin
    {
        public override string Descripton => "Combine multiple subscribers";

        public Mux(string Name, Configuration Config)
        : base(Name, Config)
        {
        }

        public override void Process(Event e)
        {
            e.Lineage.Add(Name);
            Emit(e);
        }
    }
}