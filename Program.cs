using System.Collections.Generic;
using Newtonsoft.Json;
using Hive.Core;
using System;
using CommandLine;
using System.IO;

namespace hive
{
    class Program
    {
        interface IOptions
        {
            [Option('c', "config",
                Required = false,
                Default = "config.json",
                HelpText = "Configuration file")]
            String ConfigFile { get; set; }

            [Option('p', "plugins",
                Required = false,
                Default = "./plugins",
                HelpText = "Plugins directory")]
            String PluginDirectory { get; set; }
        }

        [Verb("plugins", HelpText = "Display available plugins")]
        class ListPluginsOptions : IOptions
        {
            public String ConfigFile { get; set; }
            public String PluginDirectory  { get; set; }
        }

        [Verb("run", HelpText = "Run")]
        class RunOptions : IOptions
        {
            public String ConfigFile { get; set; }
            public String PluginDirectory  { get; set; }
        }

        static List<Configuration> LoadConfiguration(String path)
        {
            //var config = new List<Configuration>();
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                var config = (List<Configuration>)serializer.Deserialize(file, typeof(List<Configuration>));
                return config;
            }
        }

        static int ListPlugins(ListPluginsOptions opts)
        {
            var plugins = PluginFactory.GetPlugins();
            foreach (var plugin in plugins)
            {
                System.Console.WriteLine($"{plugin.Name}");
            }

            return 0;
        }

        static int Run(RunOptions opts)
        {
            var configs = LoadConfiguration(opts.ConfigFile);

            var nodes = new List<Plugin>();
            foreach (var config in configs)
            {
                var node = PluginFactory.FromConfiguration(config);
                nodes.Add(node);
            }

            foreach (var node in nodes)
            {
                var upstreams = node.Configuration.Upstreams;
                if (upstreams != null)
                {
                    foreach (var target in upstreams)
                    {
                        var upstream = nodes.Find(n => n.Name == target);
                        upstream.Subscribe(node);
                    }
                }
            }

            foreach (var node in nodes)
            {
                node.Run();
            }

            System.Console.ReadLine();

            return 0;
        }

        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<ListPluginsOptions, RunOptions>(args)
                .MapResult(
                    (ListPluginsOptions opts) => ListPlugins(opts),
                    (RunOptions opts) => Run(opts),
                    errs => 1
                );
        }
    }
}
