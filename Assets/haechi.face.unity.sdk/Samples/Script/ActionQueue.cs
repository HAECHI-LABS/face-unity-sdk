using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ActionQueue : MonoBehaviour
{
    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

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

    public void Enqueue<T>(Action<T> a, Task<T> task)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => { this.StartCoroutine(this._actionWrapper(a, task)); });
        }
    }

    private IEnumerator _actionWrapper<T>(Action<T> a, Task<T> task)
    {
        while (!task.IsCompleted)
        {
            yield return null;
        }

        a.Invoke(task.Result);
        yield return null;
    }
}