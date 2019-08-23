using System;
using CommandLine;

namespace Hive.Options
{
    public interface IOptions
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

    [Verb("validate", HelpText = "Validate configuration")]
    class ValidateOptions : IOptions
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
}