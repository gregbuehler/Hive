using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hive;

namespace Hive.Core
{
    public class Unsubscriber : IDisposable
    {
        private List<IObserver<Event>> _observers;
        private IObserver<Event> _observer;

        [DebuggerStepThroughAttribute]
        public Unsubscriber(List<IObserver<Event>> observers, IObserver<Event> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        [DebuggerStepThroughAttribute]
        public void Dispose()
        {
            if (! (_observer == null)) _observers.Remove(_observer);
        }
    }
}