using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hive.Core;

namespace Hive.Plugins
{
    public class Filter : Plugin
    {
        public override string Descripton => "Filter a stream";

        public string Key { get; set; }
        public Regex Expression { get; set; }

        public Filter(string Name, Configuration Config)
        : base(Name, Config)
        {
            this.Key = Config.Options.GetValueOrDefault("key", null);
            var pattern = Config.Options.GetValueOrDefault("expression", ".");
            this.Expression = new Regex(pattern);
        }
        public override void Process(Event e)
        {
            var value = e.Data.GetValueOrDefault(Key, "");
            if (Expression.IsMatch(e.Data.GetValueOrDefault(Key, "").ToString()))
            {
                e.Lineage.Add(Name);
                Emit(e);
            }
        }
    }
}