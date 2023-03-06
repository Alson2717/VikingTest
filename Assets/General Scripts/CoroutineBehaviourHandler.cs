using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CoroutineBehaviourHandler
{
    public CoroutineBehaviourHandler(MonoBehaviour component)
    {
        this.linkedComponent = component;
    }

    public MonoBehaviour linkedComponent;
    public Coroutine coroutine;

    public void Start(IEnumerator coroutine)
    {
        Stop();
        this.coroutine = linkedComponent.StartCoroutine(coroutine);
    }
    public void Stop()
    {
        if (this.coroutine != null)
        {
            linkedComponent.StopCoroutine(this.coroutine);
            this.coroutine = null;
        }
    }
}

