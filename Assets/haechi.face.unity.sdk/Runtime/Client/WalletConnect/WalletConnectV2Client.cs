using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using WalletConnectSharp.Common.Model.Errors;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Core.Models.Relay;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Storage;

namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class WalletConnectV2Client : MonoBehaviour, IWalletConnectClient
    {
        private Engine _engine;
        private static WalletConnectV2Client _instance;
        private WalletConnectSignClient _walletClient;
        private Queue<MessageEvent> _messageQueue = new Queue<MessageEvent>();
        
        public delegate Task PersonalSignEvent<T>(string topic, WcRequestEvent<T> @event);

        public event PersonalSignEvent<string[]> OnPersonalSignRequest;
        
        public delegate Task SendTransactionEvent<T>(string topic, WcRequestEvent<T> @event);

        public event SendTransactionEvent<SendTransaction[]> OnSendTransactionEvent;
        
        public WalletConnectSignClient Client => _walletClient;

        public static WalletConnectV2Client GetInstance()
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
                MessageEvent message = this._messageQueue.Dequeue();
                string payload = this._walletClient.Core.Crypto
                    .Decrypt(message.Topic, message.Message)
                    .Result;
                WcRequestEvent<object> json = JsonConvert.DeserializeObject<WcRequestEvent<object>>(payload);

                switch (json.Params.Request.Method)
                {
                    case "personal_sign":
                        WcRequestEvent<string[]> personalSignEvent = JsonConvert.DeserializeObject<WcRequestEvent<string[]>>(payload);
                        this.StartCoroutine(this._personalSignRequest(message.Topic, personalSignEvent));
                        break;
                    case "eth_sendTransaction":
                        WcRequestEvent<SendTransaction[]> sendTransactionEvent = JsonConvert.DeserializeObject<WcRequestEvent<SendTransaction[]>>(payload);
                        this.StartCoroutine(this._sendTransactionRequest(message.Topic,  sendTransactionEvent));
                        break;
                }
            }
        }
        
        private IEnumerator _personalSignRequest(string topic, WcRequestEvent<string[]> @event)
        {
            yield return this.OnPersonalSignRequest?.Invoke(topic, @event);
        }  
        private IEnumerator _sendTransactionRequest(string topic, WcRequestEvent<SendTransaction[]> @event)
        {
            yield return this.OnSendTransactionEvent?.Invoke(topic, @event);
        }

        public async Task<DappMetadata> RequestPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName)
        {
            return await _doPair(address, wcUri, confirmWalletConnectDapp, dappName);
        }

        private async Task<DappMetadata> _doPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName)
        {
            ProposalStruct @struct = new ProposalStruct();
            try
            {
                @struct = await this._walletClient.Pair(wcUri).WithTimeout(5000);
            }
            catch (System.Exception e)
            {
                await this._walletClient.Core.Relayer.Init();
                throw e;
            }

            DappMetadata dappMetadata = new DappMetadata(@struct.Proposer.Metadata);
            FaceRpcResponse isConfirm = await confirmWalletConnectDapp(dappMetadata);
            if (isConfirm.CastResult<bool>())
            {
                IApprovedData approveData = await this._walletClient.Approve( @struct.ApproveProposal(address));
                await approveData.Acknowledged();
                return dappMetadata;
            }
            else
            {
                await this._walletClient.Reject(new RejectParams()
                {
                    Id = @struct.Id.Value,
                    Reason = ErrorResponse.FromErrorType(ErrorType.NOT_APPROVED)
                });
                return null;
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
                Storage = new FileSystemStorage(Application.persistentDataPath + "/wc")
            };

            try
            {
                this._walletClient = await WalletConnectSignClient.Init(options);
                
                this._engine = (Engine)this._walletClient.Engine;
            
                this._engine.Events.ListenFor<MessageEvent>("request_wc_sessionRequest", (sender, @message) =>
                {
                    this._messageQueue.Enqueue(@message.EventData);
                });
            }
            catch (System.Exception e)
            {   
                Debug.Log("Wallet Connect Error");
                Debug.Log(e);
            }
        }
    }
}
