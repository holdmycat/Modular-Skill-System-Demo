using System;
using System.Collections.Generic;
using Ebonor.Framework;
using UObject = UnityEngine.Object;
namespace Ebonor.DataCtrl
{

    public static class DataEventManager
    {
        static readonly ILog log = LogManager.GetLogger(typeof(DataEventManager));

        #region EventManager(仅限客户端使用)

        static Dictionary<Type, List<Delegate>> mDicDELEvent = new Dictionary<Type, List<Delegate>>();
        private static Dictionary<Type, List<Delegate>> DicDELEvent => mDicDELEvent;

        public static void OnAttach<T>(System.Action<T> del) where T : struct
        {
            if (!mDicDELEvent.ContainsKey(typeof(T)))
            {
                mDicDELEvent.Add(typeof(T), new List<Delegate>());
            }

            if (!mDicDELEvent[typeof(T)].Contains(del))
            {
                mDicDELEvent[typeof(T)].Add(del);
            }
        }

        public static void OnDetach<T>(System.Action<T> del) where T : struct
        {
            var type = typeof(T);
            if (mDicDELEvent.ContainsKey(type))
            {
                if (mDicDELEvent[type].Contains(del))
                {
                    mDicDELEvent[type].Remove(del);
                }
            }
        }

        public static void OnValueChange<T>(T parameter) where T : struct
        {
            Type type = typeof(T);
            if (DicDELEvent.TryGetValue(type, out List<Delegate> delegates))
            {
                foreach (Delegate del in delegates)
                {
                    // You'll need to cast the Delegate back to its original type to invoke it
                    System.Action<T> callback = (System.Action<T>)del;
                    callback.Invoke(parameter);
                }
            }
        }

        public static void OnClearAllDicDELEvents()
        {
            foreach (var variable in DicDELEvent)
            {
                variable.Value.Clear();
            }

            DicDELEvent.Clear();
        }

        #endregion
    }
}
