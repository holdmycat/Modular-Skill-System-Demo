//------------------------------------------------------------
// File: UnityEngineExtention.cs
// Created: 2025-11-29
// Purpose: Utility extensions for Unity components and UI helpers.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ebonor.Framework
{
   // File: UnityEngineExtention.cs
   // Summary: Utility extensions for Unity components (fading, component helpers, coroutine helpers).
   // Note: Keep allocations low; cache component lists where possible.

   public static class UnityEngineExtention
{
   static List<Component> m_ComponentCache = new List<Component>();
   
   public static Coroutine FadeTo(this SpriteRenderer spriteRenderer, float startAlpha, float endAlpha, float duration, System.Action onComplete = null)
   {
       return spriteRenderer.gameObject.GetOrAddComponent<FadeHelper>().StartFade(spriteRenderer, startAlpha, endAlpha, duration, onComplete);
   }
   
   private class FadeHelper : MonoBehaviour
   {
       public Coroutine StartFade(SpriteRenderer spriteRenderer, float startAlpha, float endAlpha, float duration, System.Action onComplete)
       {
           return StartCoroutine(FadeCoroutine(spriteRenderer, startAlpha, endAlpha, duration, onComplete));
       }

       private IEnumerator FadeCoroutine(SpriteRenderer spriteRenderer, float startAlpha, float endAlpha, float duration, System.Action onComplete)
       {
           Color color = spriteRenderer.color;
           color.a = startAlpha;
           spriteRenderer.color = color;

           float elapsedTime = 0f;

           while (elapsedTime < duration)
           {
               elapsedTime += Time.deltaTime;
               float t = Mathf.Clamp01(elapsedTime / duration);
               color.a = Mathf.Lerp(startAlpha, endAlpha, t);
               spriteRenderer.color = color;

               yield return null;
           }

           color.a = endAlpha;
           spriteRenderer.color = color;

           onComplete?.Invoke();
           Destroy(this);
       }
   }
   
   
   /// <summary>
    /// Fade an Image from 0 to target alpha, then back to 0.
    /// </summary>
    /// <param name="image">Target Image</param>
    /// <param name="duration">Duration for each fade leg</param>
    /// <param name="targetAlpha">Destination alpha</param>
    public static void FadeInOut(this Image image, float duration, float targetAlpha)
    {
        if (image == null)
        {
            Debug.LogWarning("Image is null, cannot perform fade.");
            return;
        }

        UGUIFadeHelper helper = image.gameObject.GetOrAddComponent<UGUIFadeHelper>();
        if (helper == null)
        {
            helper = image.gameObject.AddComponent<UGUIFadeHelper>();
        }

        helper.StartFade(image, duration, targetAlpha);
    }

    /// <summary>
    /// Helper MonoBehaviour to run fade coroutines.
    /// </summary>
    private class UGUIFadeHelper : MonoBehaviour
    {
        private Coroutine currentCoroutine;

        /// <summary>
        /// Start a fade.
        /// </summary>
        public void StartFade(Image image, float duration, float targetAlpha)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }

            currentCoroutine = StartCoroutine(FadeCoroutine(image, duration, targetAlpha));
        }

        /// <summary>
        /// Fade coroutine.
        /// </summary>
        private IEnumerator FadeCoroutine(Image image, float duration, float targetAlpha)
        {
            Color color = image.color;

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                color.a = Mathf.Lerp(0f, targetAlpha, t);
                image.color = color;
                yield return null;
            }

            color.a = targetAlpha;
            image.color = color;

            elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                color.a = Mathf.Lerp(targetAlpha, 0f, t);
                image.color = color;
                yield return null;
            }

            color.a = 0f;
            image.color = color;

            currentCoroutine = null;
        }
    }
   
   
    public static T GetComponentNoAlloc<T>(this GameObject @this) where T : Component
    {
        @this.GetComponents(typeof(T), m_ComponentCache);
        Component component = m_ComponentCache.Count > 0 ? m_ComponentCache[0] : null;
        m_ComponentCache.Clear();
        return component as T;
    }
    
    public delegate void CallBack();

    public delegate void CallBackWithBool(bool b);

    public delegate void ServerCallBack(int errorcode, byte[] data);

    public static bool IsNull(this UnityEngine.Object o)
    {
        return o == null;
    }

    public static T GetOrAddComponent <T> (this GameObject _mb) where T : Component
    {
        var rel = _mb.GetComponent<T>();

        if(null == rel)
        {
            rel = _mb.AddComponent<T>();
        }

        return rel;
    }

    public static IEnumerator ProcessNextFrame (CallBack callback)
    {
        yield return null;
        callback();
    }

    public static IEnumerator ProcessNextFixedTime (CallBack callback)
    {
        yield return Time.fixedDeltaTime;
        callback();
    }
    
    public static void InvokeNextFrame (this MonoBehaviour _mb, CallBack callback)
    {
        _mb.StartCoroutine(ProcessNextFrame(callback));
    }

    public static void InvokeNextFixedTime (this MonoBehaviour _mb, CallBack callback)
    {
        _mb.StartCoroutine(ProcessNextFixedTime(callback));
    }
    
    public static UnityEngine.Coroutine InvokeSecs(this MonoBehaviour _mb, float duration, bool res, CallBackWithBool callback)
    {
        return _mb.StartCoroutine(ProcessSces(duration, res, callback));
    }
    public static IEnumerator ProcessSces(float dur, bool res, CallBackWithBool callback)
    {
        yield return new WaitForSeconds(dur);
        callback(res);
    }

    public static void LookAt2D (this Transform _tf, Transform t)
    {
        Vector3 v = new Vector3(t.position.x, _tf.position.y, t.position.z);
        _tf.LookAt(v);
    }



    public static void Log(this MonoBehaviour _mb, string debug)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.Log(debug);
        }
    }

    public static void LogFormat(this MonoBehaviour _mb, string format, params object[] args)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.LogFormat(format, args);
        }
    }

    public static void LogError(this MonoBehaviour _mb, object message)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.LogError(message);
        }
    }

    public static void LogErrorFormat(this MonoBehaviour _mb, string format, params object[] args)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.LogErrorFormat(format, args);
        }
    }

    public static void LogWarning(this MonoBehaviour _mb, object message)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.LogWarning(message);
        }
    }

    public static void LogWarningFormat(this MonoBehaviour _mb, string format, params object[] args)
    {
        if (GlobalHelper.CheckDevClient())
        {
            Debug.LogWarningFormat(format, args);
        }
    }
    
}
 
}