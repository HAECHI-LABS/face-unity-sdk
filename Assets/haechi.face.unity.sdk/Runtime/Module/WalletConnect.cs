using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nethereum.Model;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Core.Models.Relay;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models.Engine.Methods;
using WalletConnectSharp.Storage;
using JsonConvert = Unity.Plastic.Newtonsoft.Json.JsonConvert;
using Object = UnityEngine.Object;

namespace haechi.face.unity.sdk.Runtime.Module
{
    
    [Serializable]
    public class WcRequestEvent<T>
    {
        [JsonProperty("method")]
        public string Method { get; set; }
        
        [JsonProperty("id")]
        public long Id { get; set; }
        
        [JsonProperty("params")]
        public SessionRequest<T> Params { get; set;}
    }

    public class WalletConnect: MonoBehaviour
    {
        private Engine _engine;
        private static WalletConnect _instance;
        private WalletConnectSignClient _walletClient;
        private bool isConnect = false;

        private Queue<MessageEvent> messageQueue = new Queue<MessageEvent>();

        public delegate Task PersonalSignEvent<T>(string topic, WcRequestEvent<T> @event);
        private event PersonalSignEvent<string[]> _onPersonalSignRequest;
        public event PersonalSignEvent<string[]> OnPersonalSignRequest
        {
            add
            {
                _onPersonalSignRequest += value;
            }
            remove
            {
                _onPersonalSignRequest -= value;
            }
        }
        
        
        public WalletConnectSignClient wallet
        {
            get { return _walletClient; }
        }
        
        public static WalletConnect GetInstance()
        {
            return _instance;
        }

        private void Awake()
        {
            _instance = this;
        }

        public string ConvertHexToString(string hex)
        {
            hex = hex.Substring(2).Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return Encoding.ASCII.GetString(raw);;
        }

        private void Update()
        {
            if (messageQueue.Count > 0)
            {
                MessageEvent message = messageQueue.Dequeue();
                var payload = _walletClient.Core.Crypto
                    .Decrypt(message.Topic, message.Message)
                    .Result;
                var json = JsonConvert.DeserializeObject<WcRequestEvent<string[]>>(payload);

                Debug.Log($"json: {json}");
                
                if (json.Params.Request.Method.Equals("personal_sign"))
                {
                    StartCoroutine(personalSignRequest(message.Topic, json));
                }
            }
        }

        public async Task Connect()
        {
            SignClientOptions options = new SignClientOptions()
            {
                ProjectId = "fbac8c1b929e56253808ffb67c65abb0",
                Metadata = new Metadata()
                {
                    Description = "face-eddie-test",
                    Icons = new[] { "https://walletconnect.com/meta/favicon.ico" },
                    Name = "face-eddie-test",
                    Url = "https://walletconnect.com"
                },
                // Omit if you want persistant storage
                Storage = new InMemoryStorage()
                
            };

            _walletClient = await WalletConnectSignClient.Init(options);
            _engine = (Engine)_walletClient.Engine;
            
            _engine.Events.ListenFor<MessageEvent>("request_wc_sessionRequest", (sender, @message) =>
            {
                messageQueue.Enqueue(@message.EventData);
            });

            isConnect = true;
        }

        IEnumerator personalSignRequest(string topic, WcRequestEvent<string[]> @event)
        {
            yield return _onPersonalSignRequest(topic, @event);
        }
    }
}
