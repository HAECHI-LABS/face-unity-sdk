using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using WalletConnectSharpV1.Core.Events;
using WalletConnectSharpV1.Core.Models;
using WalletConnectSharpV1.Core.Models.Ethereum;
using WalletConnectSharpV1.Unity;
using WalletConnectSharpV1.Unity.Network;


namespace haechi.face.unity.sdk.Runtime.Client.WalletConnect
{
    public class WalletConnectV1Client : MonoBehaviour, IWalletConnectClient
    {
        private NativeWebSocketTransport _transport = NativeWebSocketTransport.GetInstance();
        private WalletConnectUnitySession _walletConnectUnitySession;
        private static WalletConnectV1Client _instance;
        private Queue<JsonRpcRequest> _messageQueue = new Queue<JsonRpcRequest>();

        public delegate Task PersonalSignEvent(string topic, EthPersonalSign @event);
        
        public event PersonalSignEvent OnPersonalSignRequest;

        public delegate Task SendTransactionEvent(string topic, EthSendTransaction @event);

        public event SendTransactionEvent OnSendTransactionEvent;
        
        public WalletConnectUnitySession Session
        {
            get { return this._walletConnectUnitySession; }
        }

        public static WalletConnectV1Client GetInstance()
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
                JsonRpcRequest request = this._messageQueue.Dequeue();
                
                switch (request.Method)
                {
                    case "personal_sign":
                        EthPersonalSign ethPersonalSign = (EthPersonalSign)request;
                        this.StartCoroutine(this._personalSignRequest(request.Event, ethPersonalSign));
                        break;
                    case "eth_sendTransaction":
                        EthSendTransaction ethSendTransaction = (EthSendTransaction)request;
                        this.StartCoroutine(this._sendTransactionRequest(request.Event, ethSendTransaction));
                        break;
                }
            }
        }
        
        private IEnumerator _personalSignRequest(string topic, EthPersonalSign @event)
        {
            yield return this.OnPersonalSignRequest?.Invoke(topic, @event);
        }

        private IEnumerator _sendTransactionRequest(string topic, EthSendTransaction @event)
        {
            yield return this.OnSendTransactionEvent?.Invoke(topic, @event);
        }

        public async Task<DappMetadata> RequestPair(string address, string wcUri, PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName, string dappUrl)
        {
            return await _doPair(address, dappName, dappUrl, wcUri, confirmWalletConnectDapp);
        }

        public async Task DisconnectIfSessionExist()
        {
            if (this._walletConnectUnitySession != null)
            {
                if (!this._walletConnectUnitySession.Disconnected)
                {
                    await this._walletConnectUnitySession.Disconnect();   
                }
                await this._transport.Close();
                this._transport.Dispose();
            }
        }

        private async Task<DappMetadata> _doPair(string address, string dappName, string dappUrl, string wcUri, PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp)
        {
            await this.DisconnectIfSessionExist();
            this._walletConnectUnitySession = new WalletConnectUnitySession(CreateNewSession(address, wcUri));

            TaskCompletionSource<bool> confirmCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.None);
            this._walletConnectUnitySession.OnSessionCreated += async (sender, connectSession) =>
            {
                bool isConfirm = (await confirmWalletConnectDapp(new DappMetadata(connectSession.DappMetadata))).CastResult<bool>();
                confirmCompleted.TrySetResult(isConfirm);
            };

            await this._walletConnectUnitySession.Connect();

            Task<bool> task = confirmCompleted.Task;
            if (await Task.WhenAny(task, Task.Delay(120000)) == task) {
                if (task.Result)
                {
                    await this._walletConnectUnitySession.SendConnectRequest();
                    this._walletConnectUnitySession.Events.ListenFor("personal_sign",
                        (object sender, GenericEvent<EthPersonalSign> @event) =>
                        {
                            if (this._messageQueue.All(r => r.ID != @event.Response.ID))
                            {
                                this._messageQueue.Enqueue(@event.Response);   
                            }
                        });
                    this._walletConnectUnitySession.Events.ListenFor("eth_sendTransaction",
                        (object sender, GenericEvent<EthSendTransaction> @event) =>
                        {
                            if (this._messageQueue.All(r => r.ID != @event.Response.ID))
                            {
                                this._messageQueue.Enqueue(@event.Response);   
                            }
                        });
                    return new DappMetadata(this._walletConnectUnitySession.DappMetadata);
                } 
                
                await this._walletConnectUnitySession.SendRequest(
                    new WcConnectRequest<WCSessionData>(this._walletConnectUnitySession.HandshakeId,
                        new WCSessionData()
                        {
                            peerId = this._walletConnectUnitySession.WalletId,
                            peerMeta = this._walletConnectUnitySession.WalletMetadata,
                            approved = false,
                            chainId = this._walletConnectUnitySession.ChainId,
                            networkId = this._walletConnectUnitySession.NetworkId,
                            accounts = this._walletConnectUnitySession.Accounts,
                            rpcUrl = ""
                        }), this._walletConnectUnitySession.DappPeerId, "pub", false);
            }
            return null;
        }

        private SavedSession CreateNewSession(string address, string wcUri)
        {
            string clientId = Guid.NewGuid().ToString();
            return new SavedSession(clientId, BlockchainNetworks.GetChainId(FaceSettings.Instance.Network()), wcUri,
                new[] { address },
                new ClientMeta(
                    "World's best crypto wallet", 
                    "facewallet.xyz", 
                    new[]
                    {
                        "https://api.typeform.com/responses/files/cc9ffe83aecdb26b04f57e802b173ce15cceb0811a591a292aa1846845fec01c/Group_6065.png"
                    }, 
                    "Face Wallet"));
        }
    }
}