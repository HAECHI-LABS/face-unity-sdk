using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class Wallet : IWallet
    {
        private readonly FaceRpcProvider _provider;
        private readonly FaceClient _client;
        private Dictionary<string, string> _requestIdMap = new Dictionary<string, string>();
        private string _requestId;

        internal Wallet(FaceRpcProvider provider)
        {
            this._provider = provider;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
        }

        public async Task<FaceRpcResponse> SwitchNetwork(string network)
        {
            FaceRpcRequest<int> rpcRequest = new FaceRpcRequest<int>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.wallet_switchEthereumChain, NetworkResolver.GetChainId(network));
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }
        
        public async Task<FaceRpcResponse> LoginWithCredential() 
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.face_logInSignUp);
            return await this._provider.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> IsLoggedIn()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_loggedIn);
            return await this._provider.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_logOut);
            return await this._provider.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> GetAddresses()
        {
            return await this._provider.SendFaceRpcAsync(
                new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.face_accounts));
        }

        public async Task<FaceRpcResponse> GetBalance(string account)
        {
            return await this._provider.SendFaceRpcAsync(new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_getBalance, 
                account,
                "latest"));
        }

        public async Task<TransactionRequestId> SendTransaction(RawTransaction request)
        {
            string requestId = string.Format($"unity-{Guid.NewGuid().ToString()}");
            FaceRpcRequest<object> rpcRequest =
                new FaceRpcRequest<object>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_sendTransaction, request, requestId);
            
            await this._provider.SendFaceRpcAsync(rpcRequest);
            return await this.GetTransactionRequestId(requestId);
        }

        public async Task<FaceRpcResponse> Call(RawTransaction request)
        {
            FaceRpcRequest<object> rpcRequest = new FaceRpcRequest<object>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_call, request, "latest");
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Sign(string message)
        {
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.personal_sign,
                string.Format($"0x{string.Join("", message.Select(c => ((int)c).ToString("X2")))}"));
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }
        
        public async Task<FaceRpcResponse> EstimateGas(RawTransaction transaction)
        {
            FaceRpcRequest<RawTransaction> rpcRequest =
                new FaceRpcRequest<RawTransaction>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_estimateGas, transaction);
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }
        
        private async Task<TransactionRequestId> GetTransactionRequestId(string requestId)
        {
            Debug.Log($"Request ID: {requestId}");
            Task<TransactionRequestId> task = this._client.SendHttpGetRequest<TransactionRequestId>(
                $"/api/v1/transactions/requests/{requestId}");

            try
            {
                return await task;
            }
            catch (System.Exception e)
            {
                return new TransactionRequestId(e.Message);
            }
        }

    }
}