using System;
using System.Collections.Generic;



namespace Hive.Plugins
{
    using Configuration = Dictionary<string, dynamic>;

    public abstract class Plugin : IObservable<Event>, IObserver<Event>
    {
        public string Name { get; set; }
        public abstract string Descripton { get; }

        public Configuration Configuration { get; set; }

        public Plugin(string Name, Configuration Configuration)
        {
            this.Name = Name;
            this.Configuration = Configuration;
        }

        List<IObserver<Event>> observers = new List<IObserver<Event>>();

        protected void Emit(Event e)
        {
            //System.Console.WriteLine($"{Name}.Emit() [{observers.Count} observers] => {e.ToString().Substring(0, 16)}...");

            foreach (var observer in observers)
            {
                observer.OnNext(e);
            }
        }

        public virtual void Process(Event e)
        {
            // Console.WriteLine($"{Name}.Process() => {e.ToString().Substring(0, 16)}...");

            Emit(e);
        }

        // IObservable
        public IDisposable Subscribe(IObserver<Event> observer)
        {
            if (!observers.Contains(observer))
                // Console.WriteLine($"{Name} subscribe to {observer}");
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        // IObserver
        public virtual void OnNext(Event e)
        {
            // Console.WriteLine($"{Name}.OnNext() => {e}");
            Process(e);
        }
        public virtual void OnError(Exception e) {}
        public virtual void OnCompleted() {}
    }
}