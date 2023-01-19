using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client.WalletConnect;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Core.Models.Relay;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Storage;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class WalletConnect: MonoBehaviour
    {
        private Engine _engine;
        private static WalletConnect _instance;
        private WalletConnectSignClient _walletClient;
        private bool isConnect = false;

        private Queue<MessageEvent> messageQueue = new Queue<MessageEvent>();
        private Queue<PairRequestEvent> pairRequestEventQueue = new Queue<PairRequestEvent>();
        
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

        public async Task RequestPair(string address, string wcUri, PairRequestEvent.WalletConnectPairEvent confirmWalletConnectDapp)
        {
            pairRequestEventQueue.Enqueue(new PairRequestEvent()
            {
                address = address,
                uri = wcUri,
                confirmWalletConnectDapp = confirmWalletConnectDapp
            });
        }

        private async Task _doPair(PairRequestEvent @event)
        {
            ProposalStruct @struct = await wallet.Pair(@event.uri);
            Debug.Log($"[WC] Start Pair to {@struct.Proposer.Metadata.Name}");
            Boolean isConfirm  = await @event.confirmWalletConnectDapp(@struct.Proposer.Metadata);
            Debug.Log($"[WC] isConfirm {isConfirm}");

            if (isConfirm)
            {
                var approveData = await wallet.Approve( @struct.ApproveProposal(@event.address));
                await approveData.Acknowledged();
            }
            else
            {
                await wallet.Reject(new RejectParams()
                {
                    Id = @struct.Id.Value,
                    Reason = ErrorResponse.FromErrorType(ErrorType.NOT_APPROVED)
                });
            }
        }

        private void Update()
        {
            if (messageQueue.Count > 0)
            {
                MessageEvent message = messageQueue.Dequeue();
                var payload = _walletClient.Core.Crypto
                    .Decrypt(message.Topic, message.Message)
                    .Result;
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

            if (pairRequestEventQueue.Count > 0)
            {
                PairRequestEvent @event = pairRequestEventQueue.Dequeue();
                StartCoroutine(pairWallet(@event));
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
        IEnumerator sendTransactionRequest(string topic, WcRequestEvent<SendTransaction[]> @event)
        {
            yield return _onSendTransactionEvent(topic, @event);
        }

        IEnumerator pairWallet(PairRequestEvent @event)
        {
            yield return _doPair(@event);
        }
    }
}
