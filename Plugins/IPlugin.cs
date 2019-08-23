using System;

namespace Hive.Plugins
{
    public interface IPlugin
    {
        string Name { get; set; }
        string Descripton { get; }

        IDisposable Subscribe(IObserver<Event> observer);
    }
}