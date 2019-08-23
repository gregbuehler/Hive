using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Hive.Core;

namespace Hive.Plugins
{
    using Configuration = Dictionary<string, dynamic>;

    public class PluginFactory
    {
        public static IEnumerable<Type> GetPlugins() {
            var plugins = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where( t => t.GetTypeInfo().IsSubclassOf(typeof(Plugin)));

            return plugins;
        }

        public static Plugin FromConfiguration(Configuration config)
        {
            if (!config.ContainsKey("type"))
                throw new System.Exception("No plugin type specified");

            var types = PluginFactory.GetPlugins();
            var type = config["type"];
            var name = config.GetValueOrDefault("name", $"{type}-{Guid.NewGuid()}");

            var t = types.First(x => x.Name == type);

            return Activator.CreateInstance(t, name, config);
        }
    }
}