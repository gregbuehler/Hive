using System.Collections.Generic;
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
                    { "interval", 1 },
                    { "upstreams", null }
                },
                new Configuration{
                    { "type", "RandomEvent" },
                    { "name", "web2" },
                    { "interval", 2 },
                    { "upstreams", null }
                },
                new Configuration{
                    { "type", "Mux" },
                    { "name", "mux" },
                    { "upstreams", new List<string>{ "web1", "web2" } }
                },
                new Configuration{
                    { "type", "Filter" },
                    { "name", "errors" },
                    { "key", "response-code" },
                    { "expression", @"4\d\d" },
                    { "upstreams", new List<string>{ "mux" } }
                },
                new Configuration{
                    { "type", "Stdout" },
                    { "name", "out1" },
                    { "upstreams", new List<string>{ "errors" } }
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
