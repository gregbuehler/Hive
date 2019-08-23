using System.Collections.Generic;
using Hive.Core;
using Hive.Plugins;

namespace hive
{
    using Configuration = Dictionary<string, dynamic>;

    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("Available Plugins:");
            var plugins = PluginFactory.GetPlugins();
            foreach (var plugin in plugins)
            {
                System.Console.WriteLine($"\t{plugin.Name}");
            }

            var configs = new List<Configuration>{
                new Configuration{
                    { "type", "RandomEvent" },
                    { "name", "web1" },
                    { "interval", 0.25 },
                    { "upstreams", null }
                },
                new Configuration{
                    { "type", "RandomEvent" },
                    { "name", "web2" },
                    { "interval", 1.0 },
                    { "upstreams", null }
                },
                new Configuration{
                    { "type", "Mux" },
                    { "name", "webs" },
                    { "upstreams", new List<string>{ "web1", "web2" } }
                },
                new Configuration{
                    { "type", "Filter" },
                    { "name", "errors" },
                    { "key", "response-code" },
                    { "expression", @"4\d\d" },
                    { "upstreams", new List<string>{ "webs" } }
                },
                new Configuration{
                    { "type", "Filter" },
                    { "name", "successes" },
                    { "key", "response-code" },
                    { "expression", @"2\d\d" },
                    { "upstreams", new List<string>{ "webs" } }
                },
                new Configuration{
                    { "type", "EnsureKeys" },
                    { "name", "ensure-error-key"},
                    { "Foo", new Dictionary<string, object>{{"is-error", true}} },
                    { "upstreams", new List<string>{ "errors" } }
                },
                new Configuration{
                    { "type", "Mux" },
                    { "name", "traffic" },
                    { "upstreams", new List<string>{ "successes", "ensure-error-key" } }
                },
                new Configuration{
                    { "type", "Stdout" },
                    { "name", "out1" },
                    { "upstreams", new List<string>{ "traffic" } }
                }
            };


            System.Console.WriteLine("Loading Nodes from Configs");
            var nodes = new List<Plugin>();
            foreach (var config in configs)
            {
                var node = PluginFactory.FromConfiguration(config);
                nodes.Add(node);
            }

            System.Console.WriteLine("Connecting Upstreams");
            foreach (var node in nodes)
            {
                var upstreams = node.Configuration.GetValueOrDefault("upstreams", null);
                if (upstreams != null)
                {
                    foreach (var target in upstreams)
                    {
                        var upstream = nodes.Find(n => n.Name == target);
                        upstream.Subscribe(node);
                    }
                }
            }

            System.Console.WriteLine("#yolo");
            System.Console.ReadLine();
        }
    }
}
