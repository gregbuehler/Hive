using System;
using System.Collections.Generic;
using Hive.Core;

namespace Hive.Plugins
{
    public class Stdout : Plugin
    {
        public override string Descripton => "Print to Console";

        public Stdout(string Name, Configuration Config) : base(Name, Config)
        {

        }

        public override void Process(Event e)
        {
            System.Console.WriteLine(e.ToString());
        }
    }
}