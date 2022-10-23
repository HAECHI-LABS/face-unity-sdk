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
    [RequireComponent(typeof(ActionQueue))]
    public class Face : MonoBehaviour
    {
        internal ContractDataFactory dataFactory;
        internal Wallet wallet;

        private void Awake()
        {
            FaceProviderFactory factory = new FaceProviderFactory(this.GetComponent<SafeWebviewController>());
            FaceRpcProvider client = (FaceRpcProvider)factory.CreateUnityRpcClient();
            this.dataFactory = new ContractDataFactory(new Web3(client));
            this.wallet = new Wallet(client);
        }
    }
}