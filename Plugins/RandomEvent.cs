
using System;
using System.Collections.Generic;
using System.Threading;
using Hive.Plugins;

namespace Hive.Plugins
{
    using Configuration = Dictionary<string, dynamic>;
    public class RandomEvent : Plugin
    {
        public override string Descripton => "Random web events";
        private Random prng;
        private Timer timer;
        public RandomEvent(string Name, Configuration Configuration) : base(Name, Configuration)
        {
            double interval = Configuration.GetValueOrDefault("interval", 2.0);

            this.prng = new Random();

            timer = new Timer(Run, null, TimeSpan.Zero, TimeSpan.FromSeconds(interval));
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        public override void Process(Event e)
        {
            string[] paths = {
                "/",
                "/app/foo",
                "/app/bar",
                "/app/404",
                "/about",
                "/pricing",
                "/login",
            };

            e.Lineage.Add(this.Name);
            e.Data.Add("duration", prng.Next(50, 150));
            e.Data.Add("path", paths[prng.Next(0, paths.Length)]);
            e.Data.Add("request-id", Guid.NewGuid().ToString());
            e.Data.Add("response-code", (prng.Next(0, 10) >= 9) ? 404 : 200);
            e.Data.Add("feature-x", (prng.Next(0, 10) > 7));
            e.Data.Add("feature-y", (prng.Next(0, 10) > 7));
            e.Data.Add("feature-z", (prng.Next(0, 10) > 7));

            Emit(e);
        }

        private void Run(object state)
        {
            Event e = new Event();
            Process(e);
        }
    }
}
