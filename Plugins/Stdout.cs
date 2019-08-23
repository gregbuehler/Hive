using System;
using System.Collections.Generic;

namespace Hive.Plugins
{
    using Configuration = Dictionary<string, dynamic>;

    public class Stdout : Plugin
    {
        public override string Descripton => "Print to Console";

        public Stdout(string Name, Configuration Configuration) : base(Name, Configuration)
        {

        }

        public override void Process(Event e)
        {
            System.Console.WriteLine(e.ToString());
        }
    }
}