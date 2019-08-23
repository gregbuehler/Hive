using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Hive.Core
{
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
            if (string.IsNullOrEmpty(config.Type))
            {
                throw new Exception($"Undefined node type");
            }

            var types = PluginFactory.GetPlugins();
            var t = types.First(x => x.Name == config.Type);

            return (Plugin)Activator.CreateInstance(t, config.Name, config);
        }
    }
}