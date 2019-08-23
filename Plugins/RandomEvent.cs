
using System;
using System.Collections.Generic;
using System.Threading;
using Hive.Core;
using Hive.Plugins;

namespace Hive.Plugins
{
    public class RandomEvent : Plugin
    {
        public override string Descripton => "Random web events";
        private Random prng;
        private Timer timer;

        private double interval;
        private string mode;

        private TimeSpan inf = System.Threading.Timeout.InfiniteTimeSpan;

        public RandomEvent(string Name, Configuration Config) : base(Name, Config)
        {
            this.prng = new Random();
            this.mode = Config.Options.GetValueOrDefault("mode", "structured");

            this.interval = Double.Parse(Config.Options.GetValueOrDefault("interval", "2.0").ToString());

            timer = new Timer(Run, null, inf, TimeSpan.FromSeconds(interval));
        }

        public override void Run()
        {
            timer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(interval));
        }

        private Event PopulateRaw(Event e)
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

            var ipaddr = $"{prng.Next(1, 256)}.{prng.Next(1, 256)}.{prng.Next(1, 256)}.{prng.Next(1, 256)}";
            var timeformat = "dd/MMM/yyyy:HH:mm:ss";
            var timestamp = $"{DateTime.UtcNow.ToString(timeformat)} -0700";
            var size = prng.Next(1024, 4096);
            var path = paths[prng.Next(0, paths.Length)];
            var response_code = (prng.Next(0, 10) >= 9) ? 404 : 200;

            e.Data.Add("_raw", $"{ipaddr} - - [{timestamp}] \"GET {path} HTTP/1.0\" {response_code} {size}");

            return e;
        }

        private Event PopulateStructured(Event e)
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

            e.Data.Add("remote-addr", $"{prng.Next(1, 256)}.{prng.Next(1, 256)}.{prng.Next(1, 256)}.{prng.Next(1, 256)}");
            e.Data.Add("size", prng.Next(1024, 4096));
            e.Data.Add("duration", prng.Next(50, 150));
            e.Data.Add("path", paths[prng.Next(0, paths.Length)]);
            e.Data.Add("request-id", Guid.NewGuid().ToString());
            e.Data.Add("response-code", (prng.Next(0, 10) >= 9) ? 404 : 200);
            e.Data.Add("feature-x", (prng.Next(0, 10) > 7));
            e.Data.Add("feature-y", (prng.Next(0, 10) > 7));
            e.Data.Add("feature-z", (prng.Next(0, 10) > 7));

            return e;
        }

        public override void Process(Event e)
        {
            e.Lineage.Add(this.Name);

            switch (mode)
            {
                case "structured":
                    e = PopulateStructured(e);
                    break;
                case "raw":
                    e = PopulateRaw(e);
                    break;
                default:
                    throw new Exception($"Undefined mode '{mode}'");
            }

            Emit(e);
        }

        private void Run(object state)
        {
            Event e = new Event();
            Process(e);
        }
    }
}
