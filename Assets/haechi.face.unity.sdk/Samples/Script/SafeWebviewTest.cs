using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.JsonRpc.Client.RpcMessages;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Face))]
public class SafeWebviewTest : MonoBehaviour
{
    [SerializeField] private TMP_Text responseText;

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    private Face _face;

    private void Awake()
    {
        _face = this.GetComponent<Face>();
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

    public void OnClickLogin()
    {
        Task<FaceRpcResponse> response = this._face.wallet.LoginWithCredential();
        Debug.Log(response);
        // FaceRpcRequest<string> req = new FaceRpcRequest<string>(FaceRpcMethod.face_logInSignUp);
        // this.swc.SendMessage(req.Id.ToString(), req, response =>
        // {
        //     Debug.Log($"Response ToString: {response.ToString()}");
        //     this._enqueue(() => this._handleLogin(response));
        //     
        //     return true;
        // });
    }

    private void _handleLogin(RpcResponseMessage response)
    {
        Debug.Log($"Response2 ToString: {response.ToString()}");
        this.responseText.text = response.Result.ToString();
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