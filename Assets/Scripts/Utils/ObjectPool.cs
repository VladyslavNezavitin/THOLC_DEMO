using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T : Component
{
    public readonly int initialCount;
    public bool autoExpand;

    private Transform _parent;
    private Queue<T> _objects;

    public ObjectPool(int initialCount, bool autoExpand, Transform parent = null)
    {
        _objects = new Queue<T>();
        _parent = parent;
        this.initialCount = initialCount;
        this.autoExpand = autoExpand;

        for (int i = 0; i < initialCount; i++)
            CreateAndEnqueue();
    }

    public void Recycle(T obj)
    {
        obj.gameObject.SetActive(false);
        _objects.Enqueue(obj);
    }

    public T Get()
    {
        if (!_objects.TryDequeue(out T obj))
        {
            if (autoExpand)
            {
                CreateAndEnqueue();
                obj = _objects.Dequeue();
            }
            else
            {
                Debug.LogError($"No objects available in the pool of {nameof(T)}");
            }
        }

        if (obj != null)
            obj.gameObject.SetActive(true);

        return obj;
    }

    private void CreateAndEnqueue()
    {
        T obj = new GameObject(nameof(T)).AddComponent<T>();
        obj.transform.parent = _parent;
        obj.gameObject.SetActive(false);
        
        _objects.Enqueue(obj);
    }
}
