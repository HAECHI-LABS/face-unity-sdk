using System;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Webview;
using Nethereum.Web3;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime
{
    [RequireComponent(typeof(FaceSettings))]
    [RequireComponent(typeof(SafeWebviewController))]
    public class Face : MonoBehaviour
    {
        private FaceRpcProvider _client;
        private FaceProviderFactory _factory;
        private Web3 _web3;
        internal ContractDataFactory dataFactory;
        internal Wallet wallet;

        private void Awake()
        {
            SafeWebviewController safeWebviewController = this.GetComponent<SafeWebviewController>();
            this._factory = new FaceProviderFactory(safeWebviewController);
            this._client = (FaceRpcProvider)this._factory.CreateUnityRpcClient();
            this._web3 = new Web3(this._client);
            this.dataFactory = new ContractDataFactory(this._web3);
            this.wallet = new Wallet(this._client);
        }
    }
}