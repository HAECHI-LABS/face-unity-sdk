using System;
using haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Webview;
using JetBrains.Annotations;
using Nethereum.Web3;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime
{
    [RequireComponent(typeof(SafeWebviewController))]
    public class Face : MonoBehaviour
    {
        internal ContractDataFactory dataFactory;
        internal Wallet wallet;
        internal FaceRpcProvider client;
        private SafeWebviewController _safeWebviewController; 

        public void Initialize(FaceSettings.Parameters parameters)
        {
            FaceSettings.Init(parameters);
            
            this._safeWebviewController = this.GetComponent<SafeWebviewController>();
            FaceProviderFactory factory = new FaceProviderFactory(this._safeWebviewController, FaceSettings.Instance.ServerHostURL());
            this.client = (FaceRpcProvider)factory.CreateUnityRpcClient();
            this.dataFactory = new ContractDataFactory(new Web3(this.client));
            this.wallet = new Wallet(this.client);
        }

        public void RegisterOnCloseWebviewEventHandler(Action<SafeWebviewController, CloseWebviewArgs> handler)
        {
            this._safeWebviewController.OnCloseWebview += handler;
        }
    }
}