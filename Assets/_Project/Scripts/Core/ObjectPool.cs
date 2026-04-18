using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generic recycling pool for any Component-derived prefab.
/// Pre-instantiates a number of inactive instances, hands them out
/// when Get() is called, and accepts them back via Release(). Avoids
/// the per-frame Instantiate / Destroy pattern that produces GC spikes
/// and frame-time hitches.
/// </summary>
public class ObjectPool<T> where T : Component
{
    private readonly T prefab;
    private readonly Transform parent;
    private readonly Queue<T> available = new Queue<T>();
    private readonly int maxSize;

    public int CountAvailable => available.Count;

    public ObjectPool(T prefab, int initialSize, int maxSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
        this.maxSize = maxSize;

        // Pre-warm: create the initial batch as inactive instances so
        // the first Get() doesn't pay an allocation cost.
        for (int i = 0; i < initialSize; i++)
        {
            T instance = CreateNew(false);
            available.Enqueue(instance);
        }
    }

    /// <summary>
    /// Hand out a pooled instance. Activates it before returning.
    /// If the pool is empty, allocates one new instance on demand.
    /// </summary>
    public T Get()
    {
        T instance;
        // Skip any null entries (in case something was destroyed externally).
        while (available.Count > 0)
        {
            instance = available.Dequeue();
            if (instance != null)
            {
                instance.gameObject.SetActive(true);
                return instance;
            }
        }
        return CreateNew(true);
    }

    /// <summary>
    /// Return an instance to the pool. Deactivates it. If the pool is
    /// already at maxSize, destroys the instance instead of holding it.
    /// </summary>
    public void Release(T instance)
    {
        if (instance == null) return;

        if (available.Count >= maxSize)
        {
            Object.Destroy(instance.gameObject);
            return;
        }

        instance.gameObject.SetActive(false);
        available.Enqueue(instance);
    }

    private T CreateNew(bool active)
    {
        T instance = Object.Instantiate(prefab, parent);
        instance.gameObject.SetActive(active);
        return instance;
    }
}