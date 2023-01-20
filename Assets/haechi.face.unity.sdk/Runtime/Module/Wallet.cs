using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models.Engine.Methods;
using WalletConnectSharp.Network.Models;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public interface IWallet
    {
        Task<FaceRpcResponse> GetBalance(string account = null);
    }
    
   

    public class Wallet : IWallet
    {
        private readonly FaceRpcProvider _provider;
        private readonly FaceClient _client;
        private WalletConnect _walletConnect;
        
        internal Wallet(FaceRpcProvider provider)
        {
            this._provider = provider;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
            
#if  !UNITY_WEBGL
            this._initWalletConnectV2();
#endif
        }

        /// <summary>
        /// Get balance of given account.
        /// </summary>
        /// <param name="account">Address to inquire balance.</param>
        /// <returns>Rpc call response. Result is hex string balance.</returns>
        public async Task<FaceRpcResponse> GetBalance(string account)
        {
            return await this._provider.SendFaceRpcAsync(new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_getBalance, 
                account.ToLower(),
                "latest"));
        }

        /// <summary>
        /// Send transaction with given raw transaction.
        /// </summary>
        /// <param name="request"><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.RawTransaction.html">Raw transaction</a>. This includes from, to, value, data.</param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.TransactionRequestId.html">See here</a>.</returns>
        public async Task<TransactionRequestId> SendTransaction(RawTransaction request)
        {
            string requestId = string.Format($"unity-{Guid.NewGuid().ToString()}");
            FaceRpcRequest<object> rpcRequest =
                new FaceRpcRequest<object>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_sendTransaction, request, requestId);
            
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(rpcRequest);
            return await this._getTransactionRequestId(requestId, response);
        }
        
        /// <summary>
        /// Send contract call with raw transaction data.
        /// </summary>
        /// <param name="request"><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.RawTransaction.html">Raw transaction</a>. This includes from, to, value, data.</param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is given string value from blockchain.</returns>
        public async Task<FaceRpcResponse> Call(RawTransaction request)
        {
            FaceRpcRequest<object> rpcRequest = new FaceRpcRequest<object>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_call, request, "latest");
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }

        /// <summary>
        /// Sign given message.
        /// </summary>
        /// <param name="message">Message to sign.</param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is signed message in string value.</returns>
        public async Task<FaceRpcResponse> SignMessage(string message)
        {
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.personal_sign,
                string.Format($"0x{string.Join("", message.Select(c => ((int)c).ToString("X2")))}"));
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        } 
        
        private async Task<FaceRpcResponse> _signMessage(string rawMessage)
        {
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.personal_sign, rawMessage);
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }
        
        /// <summary>
        /// Estimate gas with given transaction data.
        /// </summary>
        /// <param name="transaction"><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.RawTransaction.html">Raw transaction</a>. This includes from, to, value, data.</param>
        /// <returns>Estimated gas value in hex string format.</returns>
        public async Task<FaceRpcResponse> EstimateGas(RawTransaction transaction)
        {
            FaceRpcRequest<RawTransaction> rpcRequest =
                new FaceRpcRequest<RawTransaction>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_estimateGas, transaction);
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }

        /// <summary>
        /// Switch Face Wallet's network
        /// </summary>
        /// <param name="network">Blockchain network.</param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is given string value from blockchain.</returns>
        public async Task<FaceRpcResponse> SwitchNetwork(BlockchainNetwork network)
        {
            Blockchain originalBlockchain = FaceSettings.Instance.Blockchain();
            Blockchain switchedBlockchain = Blockchains.OfBlockchainNetwork(network);
            FaceRpcRequest<SwitchNetworkRequest> rpcRequest = new FaceRpcRequest<SwitchNetworkRequest>(originalBlockchain, FaceRpcMethod.face_switchNetwork, new SwitchNetworkRequest(switchedBlockchain.ToString()));
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(rpcRequest);
            if (!response.CastResult<string>().Equals(switchedBlockchain.ToString()))
            {
                throw new SwitchNetworkFailedException();
            }
            FaceSettings.Instance.SetNetwork(network);
            return response;
        }

        private async Task<TransactionRequestId> _getTransactionRequestId(string requestId, FaceRpcResponse response)
        {
#if UNITY_WEBGL
            Task<TransactionRequestId> task = this._provider._webRequest.SendHttpGetRequest<TransactionRequestId>(
                $"{FaceSettings.Instance.ServerHostURL()}/v1/transactions/requests/{requestId}");
#else
            Task<TransactionRequestId> task = this._client.SendHttpGetRequest<TransactionRequestId>(
                $"/v1/transactions/requests/{requestId}");
#endif

            try
            {
                TransactionRequestId transactionRequestId = await task;
                return transactionRequestId;
            }
            catch (HttpRequestException e)
            {
                if (response.IsWebviewClosed())
                {
                    throw new WebviewClosedException();
                }
                throw new FaceServerException(e);
            }
        }

        private async void _initWalletConnectV2()
        {
            this._walletConnect = WalletConnect.GetInstance();
            await this._walletConnect.Connect();
            this._registryWalletConnectEvent();
        }

        private void _registryWalletConnectEvent()
        {
            this._walletConnect.OnPersonalSignRequest += async (topic, @event) =>
            { 
                var response = await _signMessage(@event.Params.Request.Params[0]);
                _walletConnect.wallet.Respond<SessionRequest<string[]>, string>(new RespondParams<string>()
                {
                    Topic = topic,
                    Response = new JsonRpcResponse<string>()
                    {
                        Id = @event.Id,
                        Result = response.Result.ToString(),
                        Error = null
                    }
                });
            };
            this._walletConnect.OnSendTransactionEvent += async (topic, @event) =>
            {
                var response = await SendTransaction(@event.Params.Request.Params[0]);
                _walletConnect.wallet.Respond<SessionRequest<string[]>, string>(new RespondParams<string>()
                {
                    Topic = topic,
                    Response = new JsonRpcResponse<string>()
                    {
                        Id = @event.Id,
                        Result = response.transactionId,
                        Error = null
                    }
                });
            };
        }


        /// <summary>
        /// Connect Face with Opensea via WalletConnect.
        /// </summary>
        /// <param name="collectionName">Blockchain network.</param>
        public async void ConnectOpenSea(string address)
        { 
             string hostname = Profiles.IsMainNet(FaceSettings.Instance.Environment())
                ? "https://opensea.io/"
                : "https://testnets.opensea.io/";
             FaceRpcResponse response = await _openWalletConnect("OpenSea", hostname);

#if  UNITY_WEBGL
             return;
#endif
             string encodedWcUri = response.Result.Value<string>("uri");
             byte[] wcUriBytes = Convert.FromBase64String(encodedWcUri);
             string wcUri = Encoding.UTF8.GetString(wcUriBytes);
             
             this._walletConnect.RequestPair(address, wcUri, this);
        } 
        
        /// <summary>
        /// Connect Face with Dapp via WalletConnect.
        /// </summary>
        /// <param name="dappName">dapp name to connect.</param>
        /// <param name="dappUrl">dapp url to connect.</param>
        /// <param name="address">wallet address to connect.</param>
        public async void ConnectWallet(string dappName,string dappUrl, string address)
        { 
             FaceRpcRequest<String> rpcRequest = new FaceRpcRequest<String>(FaceSettings.Instance.Blockchain(), 
                 FaceRpcMethod.face_openWalletConnect, dappName, dappUrl);
             FaceRpcResponse response = await _provider.SendFaceRpcAsync(rpcRequest);
             
#if  UNITY_WEBGL
             return;
#endif
            
             string encodedWcUri = response.Result.Value<string>("uri");
             byte[] wcUriBytes = Convert.FromBase64String(encodedWcUri);
             string wcUri = Encoding.UTF8.GetString(wcUriBytes);

             this._walletConnect.RequestPair(address, wcUri, this);
        }
        
        private async Task<FaceRpcResponse> _openWalletConnect(string dappName, string dappUrl)
        {
            FaceRpcRequest<string> faceRpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_openWalletConnect, dappName, dappUrl);

            return await _provider.SendFaceRpcAsync(faceRpcRequest);
        }

        public async Task<FaceRpcResponse> ConfirmWalletConnectDapp(Metadata dappMetadata)
        {
            FaceRpcRequest<Metadata> faceRpcRequest = new FaceRpcRequest<Metadata>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_confirmWalletConnectDapp, dappMetadata);

            return await _provider.SendFaceRpcAsync(faceRpcRequest);
        }
    }
    
    /// <summary>
    /// WalletProxy 
    /// </summary>
    public class WalletProxy : IWallet
    {
        private IWallet _wallet;

        public WalletProxy()
        {
        }

        public void Register(IWallet wallet)
        {
            this._wallet = wallet;
        }
        
        public Task<FaceRpcResponse> GetBalance(string account = null)
        {
            return this._wallet.GetBalance(account);
        }
    }
    
}