using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Face))]
public class SafeWebviewTest : MonoBehaviour
{
    [SerializeField] private TMP_Text responseText;

    private Face _face;

    private void Awake()
    {
        _face = this.GetComponent<Face>();
    }

    public void OnClickLogin()
    {
        this._face.wallet.LoginWithCredential(response =>
        {
            FaceLoginResponse faceLoginResponse = response.Result.ToObject<FaceLoginResponse>();
            this.responseText.text = faceLoginResponse.faceUserId;
        });
    }
}