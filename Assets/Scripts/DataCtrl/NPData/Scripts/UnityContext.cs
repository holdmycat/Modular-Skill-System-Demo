//------------------------------------------------------------
// File: UnityContext.cs
// Created: 2025-12-05
// Purpose: Singleton Unity helper that provides shared clocks and blackboards.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;

namespace Ebonor.DataCtrl
{
    public class UnityContext : MonoBehaviour
    {
        private static UnityContext instance = null;

        private static UnityContext GetInstance()
        {
            if (instance == null)
            {
                GameObject gameObject = new GameObject();
                gameObject.name = "~Context";
                instance = (UnityContext)gameObject.AddComponent(typeof(UnityContext));
                gameObject.isStatic = true;
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
            }
            return instance;
        }

        public static Clock GetClock()
        {
            return GetInstance().clock;
        }

        public static Blackboard GetSharedBlackboard(string key)
        {
            UnityContext context = GetInstance();
            if (!context.blackboards.ContainsKey(key))
            {
                context.blackboards.Add(key, new Blackboard(context.clock));
            }
            return context.blackboards[key];
        }

        private Dictionary<string, Blackboard> blackboards = new Dictionary<string, Blackboard>();

        private Clock clock = new Clock();

        void FixedUpdate()
        {
            //clock.OnFixedUpdate(Time.deltaTime);
        }
    }
}