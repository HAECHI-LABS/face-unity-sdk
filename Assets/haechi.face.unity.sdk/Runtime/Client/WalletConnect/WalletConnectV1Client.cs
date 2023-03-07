using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using WalletConnectSharpV1.Core.Events;
using WalletConnectSharpV1.Core.Models;
using WalletConnectSharpV1.Core.Models.Ethereum;
using WalletConnectSharpV1.Core.Utils;
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
#if UNITY_IOS
        private Queue<EthPersonalSign> _termSignMessageQueue = new Queue<EthPersonalSign>();
        private Queue<DateTime> _connectRequestTimeQueue = new Queue<DateTime>();
        internal Queue<Dictionary<DateTime, NetworkMessage>> TermSignNetworkMessageQueue = new Queue<Dictionary<DateTime, NetworkMessage>>();
#endif
        public delegate Task TermSignEvent(string topic, EthPersonalSign @event);
        
        public event TermSignEvent OnTermSignRequest;
        
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

        private async void Update()
        {
#if UNITY_IOS
            if (this._termSignMessageQueue.Count > 0)
            {
                EthPersonalSign payload = this._termSignMessageQueue.Dequeue();
                this.StartCoroutine(this._termSignRequest(payload.Event, payload));
            }
#endif
            
            if (this._messageQueue.Count > 0)
            {
                JsonRpcRequest request = this._messageQueue.Dequeue();
                
                switch (request.Method)
                {
                    case "personal_sign":
                        EthPersonalSign ethPersonalSign = (EthPersonalSign)request;
                        this.StartCoroutine(this._personalSignRequest(ethPersonalSign.Event, ethPersonalSign));
                        break;
                    case "eth_sendTransaction":
                        Debug.Log("eth_sendTransaction");
                        EthSendTransaction ethSendTransaction = (EthSendTransaction)request;
                        this.StartCoroutine(this._sendTransactionRequest(ethSendTransaction.Event, ethSendTransaction));
                        break;
                }
            }
        }
        
#if UNITY_IOS
        private IEnumerator _termSignRequest(string topic, EthPersonalSign @event)
        {
            yield return this.OnTermSignRequest?.Invoke(topic, @event);
        }
#endif
        
        private IEnumerator _personalSignRequest(string topic, EthPersonalSign @event)
        {
            yield return this.OnPersonalSignRequest?.Invoke(topic, @event);
        }

        private IEnumerator _sendTransactionRequest(string topic, EthSendTransaction @event)
        {
            yield return this.OnSendTransactionEvent?.Invoke(topic, @event);
        }

        public async Task<DappMetadata> RequestPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName)
        {
            try
            {
                return await _doPair(address, wcUri, confirmWalletConnectDapp, dappName);
            }
            catch (System.Exception e)
            {
                Debug.Log($"Error message: {e.Message}");
                Debug.Log($"Error StackTrace: {e.StackTrace}");
                throw new ApplicationException("Failed to connect with dapp");
            }
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

        private async Task<DappMetadata> _doPair(string address, string wcUri, 
            PairRequestEvent.ConfirmWalletConnectDapp confirmWalletConnectDapp, string dappName)
        {
            await this.DisconnectIfSessionExist();
            this._walletConnectUnitySession = new WalletConnectUnitySession(this._createNewSession(address, wcUri));

            TaskCompletionSource<bool> confirmCompleted = new TaskCompletionSource<bool>(TaskCreationOptions.None);
            this._walletConnectUnitySession.OnSessionCreated += async (sender, connectSession) =>
            {
                bool isConfirmed = (await confirmWalletConnectDapp(new DappMetadata(connectSession.DappMetadata))).CastResult<bool>();
                Debug.Log($"[OnSessionCreated]isConfirmed?: {isConfirmed}");
                confirmCompleted.TrySetResult(isConfirmed);
            };
            
            await this._walletConnectUnitySession.Connect();

            Task<bool> task = confirmCompleted.Task;
            if (await Task.WhenAny(task, Task.Delay(120000)) == task) {
                Debug.Log($"Is connection confirmed: {task.Result}");
                if (task.Result)
                {
                    // Send connect request for pc dapp user
                    await this._walletConnectUnitySession.SendConnectRequest();
                    // Open again to avoid connection lost
                    await this._walletConnectUnitySession.Transport.Open(this._walletConnectUnitySession.Transport.URL, false);
                    
#if UNITY_IOS
                    /*
                     * Most users will be able to connect through this line,
                     * but some using Safari on iPhone may not be able to send messages.
                     * This is due to Safari's connection failures, which occur more than twice when resuming Safari,
                     * causing a reconnection delay of approximately 1 to 6 seconds.
                     */
                    await this._walletConnectUnitySession.SendConnectRequest();
                    
                    this._connectRequestTimeQueue.Enqueue(DateTime.Now);
                    expand_background_time(this.gameObject.name);
#endif

                    this._walletConnectUnitySession.Events.ListenFor("personal_sign",
                        (object sender, GenericEvent<EthPersonalSign> @event) =>
                        {
                            if (this._messageQueue.All(r => r.ID != @event.Response.ID))
                            {
                                EthPersonalSign payload = @event.Response;
#if UNITY_IOS
                                if (this._isOpenseaTermsSign(payload))
                                {
                                    this._termSignMessageQueue.Enqueue(payload);
                                    return;
                                }
#endif
                                this._messageQueue.Enqueue(payload);
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
                        }), this._walletConnectUnitySession.DappPeerId, "pub", true);
            }
            return null;
        }

#if UNITY_IOS
        private bool _isOpenseaTermsSign(EthPersonalSign payload)
        {
            if (payload.Event.Equals(ValidJsonRpcRequestMethods.PersonalSign))
            {
                string hexMessage = payload.Parameters[0];
                string plainMessage = HexByteConvertorExtensions.FromHexToPlain(hexMessage);
                if (plainMessage.Contains("Welcome to OpenSea!") 
                    && plainMessage.Contains("Click to sign in and accept the OpenSea Terms of Service: https://opensea.io/"))
                {
                    return true;
                }
            }

            return false;
        }  
#endif

        private SavedSession _createNewSession(string address, string wcUri)
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
        
#if UNITY_IOS
        
        [DllImport("__Internal")]
        extern static void expand_background_time(string objectName);
        
        [DllImport("__Internal")]
        extern static void normalize_background_time();

        [DllImport("__Internal")]
        extern static void pause_unity();

        public async void OnAdditionalFrame()
        {
            while (this._connectRequestTimeQueue.Count > 0)
            {
                DateTime connectRequestTime = this._connectRequestTimeQueue.Dequeue();
                // Timeout 30 seconds. This is for pc dapp user who will not turn the app into background mode.
                if (DateTime.Now.Subtract(connectRequestTime).TotalSeconds > 30)
                {
                    continue;
                }
                
                // Usually, browser app websocket disconnects after 1 to 6 seconds.
                // So to avoid sending message to disconnected websocket, delay 10 seconds.
                await Task.Delay(6500);
                await this._walletConnectUnitySession.SendConnectRequest();
            }
            
            while (this.TermSignNetworkMessageQueue.Count > 0)
            {
                Dictionary<DateTime, NetworkMessage> signNetworkMessage = this.TermSignNetworkMessageQueue.Dequeue();
                DateTime queuedTime = signNetworkMessage.Keys.First();
                if (DateTime.Now.Subtract(queuedTime).TotalSeconds > 30)
                {
                    continue;
                }
                
                // Usually, browser app websocket disconnects after 1 to 6 seconds.
                // So to avoid sending message to disconnected websocket, delay 10 seconds.
                await Task.Delay(6500);
                await this._walletConnectUnitySession.SendRequest(signNetworkMessage[queuedTime]);
            }
            
            Debug.Log("Pausing..\n");
            normalize_background_time();
            pause_unity();
        }
#endif
    }
}