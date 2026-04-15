using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Provides global access to shared services (GameManager, AudioManager,
/// SaveManager, ObjectPoolManager, …) without making each one a singleton.
///
/// Services register themselves on Awake; consumers resolve them on demand.
/// Compared to singletons, this:
///   - lets you swap implementations (e.g. a fake AudioManager in tests),
///   - centralises lifecycle (one place to clear everything on scene reload),
///   - makes dependencies explicit (you have to ask for a service by type).
/// </summary>
public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> _services =
        new Dictionary<Type, object>();

    /// <summary>Register a service instance under its concrete type.</summary>
    public static void Register<T>(T service) where T : class
    {
        if (service == null)
        {
            Debug.LogError(
                $"ServiceLocator: tried to register a null {typeof(T).Name}");
            return;
        }
        _services[typeof(T)] = service;
    }

    /// <summary>Look up a registered service. Returns null if missing.</summary>
    public static T Get<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out var service))
            return (T)service;

        Debug.LogWarning(
            $"ServiceLocator: no service of type {typeof(T).Name} registered");
        return null;
    }

    public static bool IsRegistered<T>() where T : class
        => _services.ContainsKey(typeof(T));

    public static void Unregister<T>() where T : class
        => _services.Remove(typeof(T));

    public static void ClearAll() => _services.Clear();
}