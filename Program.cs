using System.Collections.Generic;
using Newtonsoft.Json;
using Hive.Core;
using System;
using CommandLine;
using System.IO;
using System.Text.RegularExpressions;
using Hive.Options;

namespace Hive
{
    class Program
    {
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

        static bool ValidateConfiguration(List<Configuration> configs)
        {
            foreach(var config in configs)
            {
                if (configs.FindAll(c => c.Name == config.Name).Count > 1)
                {
                    System.Console.WriteLine($"Configuration '{config.Name}' has multiple definitions");
                    return false;
                }
            }

            return true;
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

        static int Validate(ValidateOptions opts)
        {
            var configs = LoadConfiguration(opts.ConfigFile);
            if (ValidateConfiguration(configs))
            {
                return 0;
            }

            return 2;
        }

        static int Run(RunOptions opts)
        {
            var configs = LoadConfiguration(opts.ConfigFile);
            if (!ValidateConfiguration(configs))
            {
                return 2;
            }
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
                    Console.WriteLine($"finding upstreams for {node.Name}");
                    foreach (var target in upstreams)
                    {
                        if (target == null) continue;

                        var pattern = $"^{target.Replace(@"\.", @"\.").Replace(@"\*", @".*").Replace(@"\?", ".") }$";
                        var expr = new Regex(pattern);
                        foreach(var upstream in nodes.FindAll(n => expr.IsMatch(n.Name)))
                        {
                            if (node == upstream) continue;
                            upstream.Subscribe(node);
                        }
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
            Parser.Default.ParseArguments<ListPluginsOptions, ValidateOptions, RunOptions>(args)
                .MapResult(
                    (ListPluginsOptions opts) => ListPlugins(opts),
                    (ValidateOptions opts) => Validate(opts),
                    (RunOptions opts) => Run(opts),
                    errs => 1
                );
        }
    }
}
