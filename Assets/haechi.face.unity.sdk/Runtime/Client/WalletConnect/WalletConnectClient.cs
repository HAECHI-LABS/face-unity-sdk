using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Core.Models.Pairing;
using WalletConnectSharp.Core.Models.Relay;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Storage;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class WalletConnectClient : MonoBehaviour
    {
        private Engine _engine;
        private static WalletConnectClient _instance;
        private WalletConnectSignClient _walletConnectSignClient;
        private Queue<MessageEvent> _messageQueue = new Queue<MessageEvent>();
        
        public delegate Task PersonalSignEvent<T>(string topic, JsonRpcRequest<T> @event);

        public event PersonalSignEvent<string[]> OnPersonalSignRequest;
        
        public delegate Task SendTransactionEvent<T>(string topic, JsonRpcRequest<T> @event);

        public event SendTransactionEvent<SendTransaction[]> OnSendTransactionEvent;
        
        public WalletConnectSignClient SignClient => _walletConnectSignClient;

        public static WalletConnectClient GetInstance()
        {
            return _instance;
        }

        private void Awake()
        {
            _instance = this;
        }
        
        private void Update()
        {
            if (this._messageQueue.Count > 0)
            {
                MessageEvent messageEvent = this._messageQueue.Dequeue();
                JsonRpcRequest<object[]> json = JsonConvert.DeserializeObject<JsonRpcRequest<object[]>>(messageEvent.Message);
                switch (json.Method)
                {
                    case "personal_sign":
                        JsonRpcRequest<string[]> personalSignEvent = JsonConvert.DeserializeObject<JsonRpcRequest<string[]>>(messageEvent.Message);
                        this.StartCoroutine(this._personalSignRequest(messageEvent.Topic, personalSignEvent));
                        break;
                    case "eth_sendTransaction":
                        JsonRpcRequest<SendTransaction[]> sendTransactionEvent = JsonConvert.DeserializeObject<JsonRpcRequest<SendTransaction[]>>(messageEvent.Message);
                        this.StartCoroutine(this._sendTransactionRequest(messageEvent.Topic,  sendTransactionEvent));
                        break;
                }
            }
        }

        public async Task HandleMessage()
        {
            this._engine.SessionRequestEvents<object[], object>()
                .OnRequest += (requestData) =>
            {
                var request = requestData.Request;
                var message = new MessageEvent { Topic = requestData.Topic, Message = JsonConvert.SerializeObject(request) };
                this._messageQueue.Enqueue(message);
                return Task.CompletedTask;
            };
        }

        private IEnumerator _personalSignRequest(string topic, JsonRpcRequest<string[]> @event)
        {
            yield return this.OnPersonalSignRequest?.Invoke(topic, @event);
        }  
        private IEnumerator _sendTransactionRequest(string topic, JsonRpcRequest<SendTransaction[]> @event)
        {
            yield return this.OnSendTransactionEvent?.Invoke(topic, @event);
        }

        public async Task<DappMetadata> RequestPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp)
        {
            return await _doPair(address, wcUri, confirmWalletConnectDapp);
        }

        private async Task<DappMetadata> _doPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp)
        {
#if UNITY_ANDROID
            await this.Connect();      
#endif
            await this.Connect();
            ProposalStruct @struct = new ProposalStruct();
            try
            {
                @struct = await this._walletConnectSignClient.Pair(wcUri).WithTimeout(10000);
            }
            catch (System.Exception e)
            {
                throw e;
            }

            DappMetadata dappMetadata = new DappMetadata(@struct.Proposer.Metadata);
            FaceRpcResponse isConfirm = await confirmWalletConnectDapp(dappMetadata);
            if (isConfirm.CastResult<bool>())
            {
                try
                {
                    var requiredNamespaces = @struct.RequiredNamespaces;
                    var optionalNamespaces = @struct.OptionalNamespaces;

                    var mergedNamespaces = requiredNamespaces.Concat(optionalNamespaces)
                        .GroupBy(kvp => kvp.Key)
                        .ToDictionary(
                            group => group.Key,
                            group => group.Aggregate(new ProposedNamespace(), (acc, kvp) => new ProposedNamespace
                            {
                                Chains = (acc.Chains ?? new string[0]).Concat(kvp.Value.Chains ?? new string[0]).Distinct().ToArray(),
                                Methods = (acc.Methods ?? new string[0]).Concat(kvp.Value.Methods ?? new string[0]).Distinct().ToArray(),
                                Events = (acc.Events ?? new string[0]).Concat(kvp.Value.Events ?? new string[0]).Distinct().ToArray()
                            })
                        );
                    @struct.RequiredNamespaces = new RequiredNamespaces();
                    @struct.OptionalNamespaces = mergedNamespaces;

                    IApprovedData approveData =
                        await this._walletConnectSignClient.Approve(@struct.ApproveProposal(address));
                    await approveData.Acknowledged();
                    return dappMetadata;
                }
                catch (System.Exception e)
                {
                    throw e;
                }
            }
            
            await this._walletConnectSignClient.Reject(new RejectParams()
            {
                Id = @struct.Id.Value,
                Reason = ErrorResponse.FromErrorType(ErrorType.NOT_APPROVED)
            });
            return null;
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
                Storage = new FileSystemStorage(Application.persistentDataPath + "/wc")
            };

            try
            {
                this._walletConnectSignClient = await WalletConnectSignClient.Init(options);
                
                this._engine = (Engine) this._walletConnectSignClient.Engine;
            }
            catch (System.Exception e)
            {
                if (e.Message.Contains("socket stalled"))
                {
                    await this.Connect();
                }
                Debug.Log("Wallet Connect Error");
                Debug.Log(e);
            }
        }
    }
}
