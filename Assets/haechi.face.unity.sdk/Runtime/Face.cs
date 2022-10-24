using System;
using haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Contract;
using haechi.face.unity.sdk.Runtime.Module;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Webview;
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

        private void Awake()
        {
            FaceSettings.Init(
                "bx89CFIGB12EYcSjcAmgeBRViLr4QSwfce/kCfLj7FLa9w83fh5sd7qGTjv5w8ib9Iq9jXERZD8oxAkknroVQCjlulivVgeLn7wI6Pg0hQiAKWG9GSpvcXpqUpkL1bzNZKNfZNulMlxws6OkVFqbmUHoX4VF1TXrDSZeQetPjK4u4pJH/NosXFn1CaVFCHneM7wc/9ry9p0MmNhXe5t9Nai6UD4JlLyheW8MIuxqTXU=",
                "Dev", "ETHEREUM");
            
            SafeWebviewController safeWebviewController = this.GetComponent<SafeWebviewController>();
            FaceProviderFactory factory = new FaceProviderFactory(safeWebviewController, FaceSettings.Instance.ServerHostURL());
            this.client = (FaceRpcProvider)factory.CreateUnityRpcClient();
            this.dataFactory = new ContractDataFactory(new Web3(this.client));
            this.wallet = new Wallet(this.client);
        }
    }
}