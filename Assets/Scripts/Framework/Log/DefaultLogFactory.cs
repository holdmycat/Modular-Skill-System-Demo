//------------------------------------------------------------
// File: DefaultLogFactory.cs
// Created: 2025-11-29
// Purpose: Default implementation of the logging factory.
// Author: Xuefei Zhao (clashancients@gmail.com)
//------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Ebonor.Framework
{
    public class DefaultLogFactory : ILogFactory
    {
        private Dictionary<string, ILog> repositories = new Dictionary<string, ILog>();

        private Level _level = Level.ALL;
        
        bool _isDev = false;

        // File Logging Support
        public bool EnableFileLog { get; set; }
        private string _logFilePath;

        public DefaultLogFactory()
        {
        }

        public Level Level
        {
            get { return this._level; }
            set { this._level = value; }
        }

        public bool InUnity
        {
            get
            {
                bool isMainThread = GlobalHelper.OnIsInMainThread();  
                
#if UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX || UNITY_STANDALONE_LINUX || SERVER
                _isDev = true;
#elif UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                _isDev = isMainThread;
#else
            
                _isDev = isMainThread?Debug.isDebugBuild : false;
#endif
                return _isDev && _level != Level.OFF;
            }
        }

        public void WriteToFile(string logName, string level, object message, Exception exception = null)
        {
            if (!EnableFileLog) return;

            try
            {
                if (string.IsNullOrEmpty(_logFilePath))
                {
                    // Use project root directory (outside Assets)
                    string projectRoot = Directory.GetParent(Application.dataPath).FullName;
                    string dir = Path.Combine(projectRoot, "Logs");
                    
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    // Format: Log_yyyy-MM-dd_HH-mm.txt
                    string filename = $"Log_{DateTime.Now:yyyy-MM-dd_HH-mm}.txt";
                    _logFilePath = Path.Combine(dir, filename);
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                sb.Append(" [");
                sb.Append(level);
                sb.Append("] ");
                sb.Append(logName);
                sb.Append("-");
                sb.Append(message);
                if (exception != null)
                {
                    sb.Append(" Exception:");
                    sb.Append(exception);
                }
                sb.AppendLine();

                File.AppendAllText(_logFilePath, sb.ToString());
            }
            catch (Exception)
            {
                // Put fail-safe here to prevent infinite error loops if logging fails
            }
        }

        public ILog GetLogger<T>()
        {
            return GetLogger(typeof(T));
        }

        public ILog GetLogger(Type type)
        {
            ILog log;
            if (repositories.TryGetValue(type.FullName, out log))
                return log;

            log = new LogImpl(type.Name, this);
            repositories[type.FullName] = log;
            return log;
        }

        public ILog GetLogger(string name)
        {
            ILog log;
            if (repositories.TryGetValue(name, out log))
                return log;

            log = new LogImpl(name, this);
            repositories[name] = log;
            return log;
        }

        public void SetLogSwitch(bool on)
        {
            this._level = on ? Level.ALL : Level.OFF;
        }

        public bool GetLogState()
        {
            return this._level == Level.ALL;
        }
    }

    internal class LogImpl : ILog
    {
        private string name;
        private DefaultLogFactory _factory;
        public LogImpl(string name, DefaultLogFactory factory)
        {
            this.name = name;
            this._factory = factory;
        }
            
        public string Name { get { return this.name; } }

        protected virtual string Format(object message, string level)
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            string formattedTime = now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            return $"{formattedTime} [{level}] {name}-{message}";
                
            //return string.Format("{0:yyyy-MM-dd HH:mm:ss.fff} [{1}] {2}-{3}", System.DateTime.Now, level, name, message);
        }

        public virtual void Debug(object message)
        {
            if (!IsEnabled(Level.DEBUG))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.Log(Format(message, "DEBUG"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "DEBUG"));
#endif
            _factory.WriteToFile(name, "DEBUG", message);
        }

        public virtual void Debug(object message, Exception exception)
        {
            if (!IsEnabled(Level.DEBUG))
            {
                return;
            }

            Debug($"{message} Exception:{exception}");
            // WriteToFile is called inside Debug(string) or we handle exception explicit logic?
            // The original code calls Debug(string), which calls the overload above.
            // But let's be safe. The original Debug calls Debug(string). 
        }

        public virtual void DebugAssertionFormat(object message)
        {
            if (!IsEnabled(Level.DEBUG))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.LogAssertionFormat(Format(message, "ERROR"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "ERROR"));
#endif
            _factory.WriteToFile(name, "ERROR", message);
        }

        public virtual void DebugFormat(string format, params object[] args)
        {
            if (!IsEnabled(Level.DEBUG))
            {
                return;
            }

            Debug(string.Format(format, args));
        }
            
        public virtual void Info(object message)
        {
            if (!IsEnabled(Level.INFO))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.Log(Format(message, "INFO"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "INFO"));
#endif
            _factory.WriteToFile(name, "INFO", message);
        }

        public virtual void Info(object message, Exception exception)
        {
            if (!IsEnabled(Level.INFO))
            {
                return;
            }

            Info(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void InfoFormat(string format, params object[] args)
        {
            if (!IsEnabled(Level.INFO))
            {
                return;
            }

            Info(string.Format(format, args));
        }

        public virtual void Warn(object message)
        {
            if (!IsEnabled(Level.WARN))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.LogWarning(Format(message, "WARN"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "WARN"));
#endif
            _factory.WriteToFile(name, "WARN", message);
        }

        public virtual void Warn(object message, Exception exception)
        {
            if (!IsEnabled(Level.WARN))
            {
                return;
            }

            //Warn(string.Format("{0} Exception:{1}", message, exception));
            // Original code was commented out? Let's respect original but ensure file log if uncommented logic exists.
            // Wait, original: //Warn(...) 
            // So it does nothing? Then no file log.
        }

        public virtual void WarnFormat(string format, params object[] args)
        {
            if (!IsEnabled(Level.WARN))
            {
                return;
            }

            Warn(string.Format(format, args));
        }

        public virtual void Error(object message)
        {
            if (!IsEnabled(Level.ERROR))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.LogError(Format(message, "ERROR"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "ERROR"));
#endif
            _factory.WriteToFile(name, "ERROR", message);
        }

        public virtual void Error(object message, Exception exception)
        {
            if (!IsEnabled(Level.ERROR))
            {
                return;
            }

            Error(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void ErrorFormat(string format, params object[] args)
        {
            if (!IsEnabled(Level.ERROR))
            {
                return;
            }

            Error(string.Format(format, args));
        }

        public virtual void Fatal(object message)
        {
            if (!IsEnabled(Level.FATAL))
            {
                return;
            }

            if (this._factory.InUnity)
                UnityEngine.Debug.LogError(Format(message, "FATAL"));
#if !NETFX_CORE
            else
                Console.WriteLine(Format(message, "FATAL"));
#endif
            _factory.WriteToFile(name, "FATAL", message);
        }

        public virtual void Fatal(object message, Exception exception)
        {
            if (!IsEnabled(Level.FATAL))
            {
                return;
            }

            Fatal(string.Format("{0} Exception:{1}", message, exception));
        }

        public virtual void FatalFormat(string format, params object[] args)
        {
            if (!IsEnabled(Level.FATAL))
            {
                return;
            }

            Fatal(string.Format(format, args));
        }

        protected bool IsEnabled(Level level)
        {
            return level >= this._factory.Level;
        }

        public virtual bool IsDebugEnabled
        {
            get { return IsEnabled(Level.DEBUG); }
        }

        public virtual bool IsInfoEnabled
        {
            get { return IsEnabled(Level.INFO); }
        }

        public virtual bool IsWarnEnabled
        {
            get { return IsEnabled(Level.WARN); }
        }

        public virtual bool IsErrorEnabled
        {
            get { return IsEnabled(Level.ERROR); }
        }

        public virtual bool IsFatalEnabled
        {
            get { return IsEnabled(Level.FATAL); }
        }
    }
}
