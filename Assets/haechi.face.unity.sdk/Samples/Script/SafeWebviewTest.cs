using System;
using System.Collections;
using System.Collections.Generic;
using haechi.face.unity.sdk.Runtime.Webview;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SafeWebviewTest : MonoBehaviour
{
    [SerializeField] private SafeWebviewController swc;

    [SerializeField] private TMP_Text responseText;

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

    public void OnClickLogin()
    {
        string id = Random.Range(1, 10000).ToString();
        this.swc.Request(id, "/login", response =>
        {
            Debug.Log($"Response ToString: {response.ToString()}");
            this._enqueue(() => this._handleLogin(response));
            
            return true;
        });
    }

    private void _handleLogin(TestResponse response)
    {
        Debug.Log($"Response2 ToString: {response.ToString()}");
        this.responseText.text = response.ToString();
    }
    
    private void _enqueue(Action action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() => {
                StartCoroutine(ActionWrapper(action));
            });
        }
    }

    private IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }
}