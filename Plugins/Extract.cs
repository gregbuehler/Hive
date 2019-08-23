using System.Collections.Generic;
using System.Text.RegularExpressions;
using Hive.Core;

namespace Hive.Plugins
{
    public class Extract : Plugin
    {
        public override string Descripton => "Extract facets";

        public string Key { get; set; }
        public Regex Expression { get; set; }

        public Extract(string Name, Configuration Config)
        : base(Name, Config)
        {
            this.Key = Config.Options.GetValueOrDefault("key", "");
            var pattern = Config.Options.GetValueOrDefault("expression", ".");
            this.Expression = new Regex(pattern);
        }
        public override void Process(Event e)
        {
            e.Lineage.Add(Name);

            var value = e.Data.GetValueOrDefault(Key, "").ToString();
            var m = Expression.Match(value);

            e.Data.Add("extraction-success", m.Success);

            if (m.Success)
            {
                for (int idx = 0; idx < m.Groups.Count; idx++)
                {
                    if (!double.TryParse(m.Groups[idx].Name, out _))
                    {
                        e.Data.Add(m.Groups[idx].Name, m.Groups[idx].Value);
                    }
                }
            }

            Emit(e);
        }
    }
}