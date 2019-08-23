using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hive.Plugins
{
    public class Filter : Plugin
    {
        public override string Descripton => "Filter a stream";

        public string Key { get; set; }
        public Regex Expression { get; set; }

        public Filter(string Name, System.Collections.Generic.Dictionary<string, dynamic> Configuration)
        : base(Name, Configuration)
        {
            this.Key = Configuration.GetValueOrDefault("key", null);
            var pattern = Configuration.GetValueOrDefault("expression", ".");
            this.Expression = new Regex(pattern);
        }
        public override void Process(Event e)
        {
            var value = e.Data.GetValueOrDefault(Key, "");
            System.Console.WriteLine($"Filter; key={Key}\tvalue={value}");
            if (Expression.IsMatch(e.Data.GetValueOrDefault(Key, "").ToString()))
            {
                e.Lineage.Add(Name);
                Emit(e);
            }
        }
    }
}