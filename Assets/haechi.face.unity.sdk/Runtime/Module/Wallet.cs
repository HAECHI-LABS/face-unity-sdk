using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;

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

        internal Wallet(FaceRpcProvider provider)
        {
            this._provider = provider;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
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

        public async Task<FaceRpcResponse> OpenHome()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.face_openHome);
            return await this._provider.SendFaceRpcAsync(request);
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
            Task<TransactionRequestId> task = this._provider.WebRequest.SendHttpGetRequest<TransactionRequestId>(
                $"/v1/transactions/requests/{requestId}");
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