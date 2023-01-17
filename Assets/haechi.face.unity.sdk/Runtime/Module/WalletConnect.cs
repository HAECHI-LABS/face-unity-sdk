using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client.Face;
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
        
        public delegate Task SendTransactionEvent<T>(string topic, WcRequestEvent<T> @event);
        private event SendTransactionEvent<SendTransaction[]> _onSendTransactionEvent;
        public event SendTransactionEvent<SendTransaction[]> OnSendTransactionEvent
        {
            add
            {
                _onSendTransactionEvent += value;
            }
            remove
            {
                _onSendTransactionEvent -= value;
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
                Debug.Log($"json: {payload}");
                
                var json = JsonConvert.DeserializeObject<WcRequestEvent<object>>(payload);


                switch (json.Params.Request.Method)
                {
                    case "personal_sign":
                        var personalSignEvent = JsonConvert.DeserializeObject<WcRequestEvent<string[]>>(payload);
                        StartCoroutine(personalSignRequest(message.Topic, personalSignEvent));
                        break;
                    case "eth_sendTransaction":
                        var sendTransactionEvent = JsonConvert.DeserializeObject<WcRequestEvent<SendTransaction[]>>(payload);
                        StartCoroutine(sendTransactionRequest(message.Topic,  sendTransactionEvent));
                        break;
                }
            }
        }

        public async Task Connect()
        {
            SignClientOptions options = new SignClientOptions()
            {
                ProjectId = "5d868db873762d9d13d736cd29324fb0",
                Metadata = new Metadata()
                {
                    Description = "Face Wallet Dev",
                    Icons = new[] { "https://framerusercontent.com/images/nwVboVsol0AjcTPcRr0gdqlQODk.png" },
                    Name = "Face Wallet Dev",
                    Url = "https://facewallet.xyz/"
                },
                // Omit if you want persistant storage
                Storage = new FileSystemStorage()
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
        IEnumerator sendTransactionRequest(string topic, WcRequestEvent<SendTransaction[]> @event)
        {
            yield return _onSendTransactionEvent(topic, @event);
        }
    }
}
