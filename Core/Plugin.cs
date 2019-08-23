using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hive.Core
{
    public abstract class Plugin : IObservable<Event>, IObserver<Event>
    {
        public string Name { get; set; }
        public abstract string Descripton { get; }
        public Configuration Configuration { get; set; }

        List<IObserver<Event>> observers = new List<IObserver<Event>>();


        public Plugin(string Name, Configuration Configuration)
        {
            this.Name = Name;
            this.Configuration = Configuration;
        }

        // IObservable
        [DebuggerStepThroughAttribute]
        public IDisposable Subscribe(IObserver<Event> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);

            return new Unsubscriber(observers, observer);
        }

        // IObserver
        [DebuggerStepThroughAttribute]
        public virtual void OnNext(Event e)
        {
            Process(e);
        }

        [DebuggerStepThroughAttribute]
        public virtual void OnError(Exception e) {}

        [DebuggerStepThroughAttribute]
        public virtual void OnCompleted() {}

        [DebuggerStepThroughAttribute]
        protected void Emit(Event e)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(e);
            }
        }

        public virtual void Process(Event e)
        {
            Emit(e);
        }

        public virtual void Run()
        {

        }
    }
}