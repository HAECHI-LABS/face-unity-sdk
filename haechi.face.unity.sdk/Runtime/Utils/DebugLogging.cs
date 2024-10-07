using System;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Utils
{
    public static class DebugLogging
    {
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error
        }

        public static LogLevel logLevel = LogLevel.Info;

        public static void DebugLog(string message)
        {
            if (logLevel == LogLevel.Debug)
            {
                UnityEngine.Debug.Log(message);
            }
        }

        internal static void DebugError(System.Exception e)
        {
            if (logLevel == LogLevel.Debug)
            {
                UnityEngine.Debug.LogError(e);
            }
        }
    }
}