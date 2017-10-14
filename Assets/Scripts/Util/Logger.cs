using UnityEngine;
using System;

// Simple utility class to log objects with a context and a timestamp.
// Supports logging info, warnings, and errors.
public static class Logger {
    
    public static void Log(object obj, UnityEngine.Object context = null) {
        Debug.Log(String.Concat(System.DateTime.UtcNow.ToString(
            "[yyyy-MM-dd HH:mm:ss.fff] "), obj), context);
    }

    public static void LogWarning(object obj, UnityEngine.Object context = null) {
		Debug.LogWarning(String.Concat(System.DateTime.UtcNow.ToString(
				"[yyyy-MM-dd HH:mm:ss.fff] "), obj), context);
    }

	public static void LogError(object obj, UnityEngine.Object context = null)
	{
        Debug.LogError(String.Concat(System.DateTime.UtcNow.ToString(
				"[yyyy-MM-dd HH:mm:ss.fff] "), obj), context);
	}
}
