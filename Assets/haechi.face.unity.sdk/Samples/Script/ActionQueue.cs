using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();
    public static ActionQueue Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    
    private void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    public void Enqueue<T>(Task<T> task, Action<T> a, Action<Exception> e)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(_actionWrapper(task, a, e));
            });
        }
    }

    private IEnumerator _actionWrapper<T>(Task<T> task, Action<T> a, Action<Exception> e)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        if (!task.IsCompletedSuccessfully)
        {
            e.Invoke(task.Exception);
        }
        else
        {
            a.Invoke(task.Result);
        }
        yield return null;
    }
}   
