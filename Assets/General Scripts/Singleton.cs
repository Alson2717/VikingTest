using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get;
        protected set;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            DestroyImmediate(gameObject);
            return;
        }
        Instance = GetComponent<T>();
        SingletonAwake();
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            SingletonDestroy();
            Instance = null;
        }
    }

    protected abstract void SingletonAwake();
    protected abstract void SingletonDestroy();
}

