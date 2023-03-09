using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pool<T> where T : Component
{
    private List<T> pool = new List<T>();

    public int Count
    {
        get { return pool.Count; }
    }

    /// <summary>
    /// Stores element in pool
    /// </summary>
    public void Add(T notPrefab)
    {
        pool.Add(notPrefab);
    }
    /// <summary>
    /// Stores array of elements in pool
    /// </summary>
    public void Add(params T[] notPrefabs)
    {
        pool.AddRange(notPrefabs);
    }
    /// <summary>
    /// Stores collection of elements in pool
    /// </summary>
    public void Add(IEnumerable<T> notPrefabs)
    {
        pool.AddRange(notPrefabs);
    }
    /// <summary>
    /// Creates desired amount of elements from prefab and stores them
    /// </summary>
    public T[] Add(T prefab, int howMuch, Scene bindTo = default)
    {
        T[] elements = new T[howMuch];
        for (int i = 0; i < howMuch; i++)
        {
            elements[i] = GameObject.Instantiate(prefab);
            if (bindTo != default)
                SceneManager.MoveGameObjectToScene(elements[i].gameObject, bindTo);
        }
        Add(elements);
        return elements;
    }
    /// <summary>
    /// Removes element from pool
    /// </summary>
    public bool Remove(T notPrefab)
    {
        return pool.Remove(notPrefab);
    }


    /// <summary>
    /// If contains > 0 elements returns first element of the pool, otherwise creates new element from prefab, stores it, binds it to scene(if set) and returns it
    /// </summary>
    public T First(T prefab, Scene bindTo = default)
    {
        if (pool.Count == 0)
        {
            T component = GameObject.Instantiate(prefab);
            Add(component);
            if (bindTo != default)
                SceneManager.MoveGameObjectToScene(component.gameObject, bindTo);
            return component;
        }
        else
            return pool[0];
    }
    /// <summary>
    /// If contains > 0 elements returns random element of the pool, otherwise creates new element from prefab, stores it, binds it to scene(if set) and returns it
    /// </summary>
    public T Random(T prefab, Scene bindTo = default)
    {
        if (pool.Count == 0)
        {
            T component = GameObject.Instantiate(prefab);
            Add(component);
            if (bindTo != default)
                SceneManager.MoveGameObjectToScene(component.gameObject, bindTo);
            return component;
        }
        else
            return pool.Random();
    }
    /// <summary>
    /// If contains > 0 elements extracts first element of the pool, otherwise created new element from prefab, binds it to scene(if set) and returns it
    /// </summary>
    public T ExtractFirst(T prefab, Scene bindTo = default)
    {
        if (pool.Count == 0)
        {
            T component = GameObject.Instantiate(prefab);
            if (bindTo != default)
                SceneManager.MoveGameObjectToScene(component.gameObject, bindTo);
            return component;
        }
        else
            return pool.ExtractAt(0);
    }
    /// <summary>
    /// If contains > 0 elements extracts random element of the pool, otherwise created new element from prefab, binds it to scene(if set) and returns it
    /// </summary>
    public T ExtractRandom(T prefab, Scene bindTo = default)
    {
        if (pool.Count == 0)
        {
            T component = GameObject.Instantiate(prefab);
            if (bindTo != default)
                SceneManager.MoveGameObjectToScene(component.gameObject, bindTo);
            return component;
        }
        else
            return pool.ExtractRandom();
    }
    /// <summary>
    ///  If contains element suitable for specified predicate will extract that element, otherwise creates new element from prefab, binds it to scene(if set) and returns it
    /// </summary>
    public T ExtractFind(System.Predicate<T> predicate, T prefab, Scene bindTo = default)
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (predicate(pool[i]))
                return pool.ExtractAt(i);
        }

        T component = GameObject.Instantiate(prefab);
        if (bindTo != default)
            SceneManager.MoveGameObjectToScene(component.gameObject, bindTo);
        return component;
    }

    public void ClearAndDestroy()
    {
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i] != null)
                GameObject.Destroy(pool[i].gameObject);
        }
        pool.Clear();
    }


    public IEnumerator GetEnumerator()
    {
        return pool.GetEnumerator();
    }

    public T this[int index]
    {
        get { return pool[index]; }
        set { pool[index] = value; }
    }
}

