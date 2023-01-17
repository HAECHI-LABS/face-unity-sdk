using System;
using System.Buffers.Text;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using JetBrains.Annotations;
using Nethereum.ABI.Util;
using Nethereum.Unity.Rpc;
using UnityEngine;
using WalletConnectSharp.Common.Utils;
using WalletConnectSharp.Network.Models;
using WalletConnectSharp.Sign;
using WalletConnectSharp.Sign.Models;
using WalletConnectSharp.Sign.Models.Engine;
using WalletConnectSharp.Sign.Models.Engine.Methods;
using static Org.BouncyCastle.Utilities.Encoders.Base64;
using Task = UnityEditor.VersionControl.Task;

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
            _initWalletConnectV2();
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
                account,
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
            Blockchain blockchain = FaceSettings.Instance.Blockchain();
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(blockchain, FaceRpcMethod.face_switchNetwork, blockchain.ToString());
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(rpcRequest);
            FaceSettings.Instance.SetNetwork(network);
            return response;
        }
        
        private async Task<TransactionRequestId> _getTransactionRequestId(string requestId, FaceRpcResponse response)
        {
            Task<TransactionRequestId> task = this._client.SendHttpGetRequest<TransactionRequestId>(
                $"/v1/transactions/requests/{requestId}");
            
            try
            {
                return await task;
            }
            catch (HttpRequestException e)
            {
                if (response.IsWebviewClosed())
                {
                    throw new WebviewClosedException();
                }
                throw new FaceException(ErrorCodes.SERVER_RESPONSE_ERROR, e.Message);
            }
        }

        public async void _initWalletConnectV2()
        {
            this._walletConnect = WalletConnect.GetInstance();
            await this._walletConnect.Connect();
            _walletConnect.OnPersonalSignRequest += async (topic, @event) =>
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
 
            _walletConnect.OnSendTransactionEvent += async (topic, @event) =>
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


        public async void ConnectOpenSea(string address)
        { 
            string hostname = Profiles.IsMainNet(FaceSettings.Instance.Environment())
                ? "https://opensea.io/"
                : "https://testnets.opensea.io/";
             
             FaceRpcRequest<String> rpcRequest = new FaceRpcRequest<String>(FaceSettings.Instance.Blockchain(), 
                 FaceRpcMethod.face_openWalletConnect, "OpenSea", hostname);
             FaceRpcResponse response = await _provider.SendFaceRpcAsync(rpcRequest);
             Debug.Log(response);
             
             await _openWalletConnectWithTopic(response.Result.Value<string>("topic"));
        }
        
        
        /// <summary>
        /// Connect Face with Opensea via WalletConnect.
        /// </summary>
        /// <param name="collectionName">Blockchain network.</param>
        public async void ConnectWallet(string address, string wcUrl, [CanBeNull] string collectionName = null)
        {
            // string hostname = Profiles.IsMainNet(FaceSettings.Instance.Environment())
            //     ? "https://opensea.io/"
            //     : "https://testnets.opensea.io/";
            //  FaceRpcResponse response = await this._openWalletConnect("OpenSea",
            //     !string.IsNullOrEmpty(collectionName)
            //         ? $"{hostname}/collection/" + collectionName
            //         : $"{hostname}");
            //
            //  Debug.Log("connect opensea" + response.ToString());
            //  
            //
            //  Debug.Log(response.Result.ToString());
            //  
             await _openWalletConnect(address, wcUrl);
        }

        private async Task<WalletConnect> _openWalletConnect(string address, string wcUrl)
        {
            WalletConnectSignClient wallet = _walletConnect.wallet;
            Debug.Log("[WC] start _openWalletConnect");

            ProposalStruct @struct = await wallet.Pair(wcUrl);
            Debug.Log("[WC] Pair");
            var approveData = await wallet.Approve( @struct.ApproveProposal(address));
            Debug.Log($"[WC] Approve {approveData.Topic}");
            await approveData.Acknowledged();
            Debug.Log("[WC] Acknowledged");

            
            
            return _walletConnect;
        }
        private async Task<WalletConnect> _openWalletConnectWithTopic(string topic)
        {
            WalletConnectSignClient wallet = _walletConnect.wallet;
            Debug.Log(wallet.Session.Context);

            wallet.Engine.Client.Connect(new ConnectOptions().WithPairingTopic(topic));
            
            await wallet.Engine.Client.Pairing.Update(topic, new PairingStruct()
            {
                Active = true,
                Expiry = Clock.CalculateExpiry(Clock.THIRTY_DAYS)
            });
            
            wallet.Core.Relayer.Subscribe(topic);
            Debug.Log("[WC] subscribe topic");

            return _walletConnect;
        }

        // private async Task<FaceRpcResponse> _openWalletConnect(String name, String url)
        // {
        //     FaceRpcRequest<String> rpcRequest = new FaceRpcRequest<String>(FaceSettings.Instance.Blockchain(), 
        //         FaceRpcMethod.face_openWalletConnect, name, url);
        //     
        //     return await _provider.SendFaceRpcAsync(rpcRequest);
        // }
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