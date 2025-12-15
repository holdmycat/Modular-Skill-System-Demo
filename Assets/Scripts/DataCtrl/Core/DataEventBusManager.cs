using System;
using System.Collections.Generic;
using Ebonor.Framework;

namespace Ebonor.DataCtrl
{
    public class DataEventBusManager : IDataEventBus, IDisposable
    {
        static readonly ILog log = LogManager.GetLogger(typeof(DataEventBusManager));

        private Dictionary<Type, List<Delegate>> _dicDelEvent;
        
        public DataEventBusManager()
        {
            _dicDelEvent = new Dictionary<Type, List<Delegate>>();
        }
        
        public void OnAttach<T>(System.Action<T> del) where T : struct
        {
            if (!_dicDelEvent.ContainsKey(typeof(T)))
            {
                _dicDelEvent.Add(typeof(T), new List<Delegate>());
            }

            if (!_dicDelEvent[typeof(T)].Contains(del))
            {
                _dicDelEvent[typeof(T)].Add(del);
            }
        }

        public void OnDetach<T>(System.Action<T> del) where T : struct
        {
            var type = typeof(T);
            if (_dicDelEvent.ContainsKey(type))
            {
                if (_dicDelEvent[type].Contains(del))
                {
                    _dicDelEvent[type].Remove(del);
                }
            }
        }

        public void OnValueChange<T>(T parameter) where T : struct
        {
            Type type = typeof(T);
            if (_dicDelEvent.TryGetValue(type, out List<Delegate> delegates))
            {
                foreach (Delegate del in delegates)
                {
                    // You'll need to cast the Delegate back to its original type to invoke it
                    System.Action<T> callback = (System.Action<T>)del;
                    callback.Invoke(parameter);
                }
            }
        }

        public void OnClearAllDicDELEvents()
        {
            foreach (var variable in _dicDelEvent)
            {
                variable.Value.Clear();
            }

            _dicDelEvent.Clear();
        }


        public void Dispose()
        {
            OnClearAllDicDELEvents();
        }
    }
}


