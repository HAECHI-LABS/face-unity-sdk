using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Settings;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using Nethereum.JsonRpc.Client.RpcMessages;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class Wallet
    {
        private readonly FaceRpcProvider _client;

        internal Wallet(FaceRpcProvider client)
        {
            this._client = client;
        }

        public async Task<FaceRpcResponse> InitializeFaceSdk(FaceEnvironments env)
        {
            FaceRpcRequest<FaceEnvironments> rpcRequest =
                new FaceRpcRequest<FaceEnvironments>(FaceSettings.Instance.Blockchain(), 
                    FaceRpcMethod.wallet_initialize, env);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> SwitchNetwork(string network)
        {
            FaceRpcRequest<int> rpcRequest = new FaceRpcRequest<int>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.wallet_switchEthereumChain, NetworkResolver.GetChainId(network));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }
        
        public Task<FaceRpcResponse> LoginWithCredential() 
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.face_logInSignUp);
            return this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> IsLoggedIn()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_loggedIn);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_logOut);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> GetAddresses()
        {
            return await this._client.SendFaceRpcAsync(
                new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.face_accounts));
        }

        public async Task<FaceRpcResponse> GetBalance(string account = null)
        {
            // TODO: Get address from the cache
            return await this._client.SendFaceRpcAsync(new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_getBalance, 
                "0xDD9724Ecd92487633EC0191Ba7737009127D260e",
                "latest"));
        }

        public async Task<FaceRpcResponse> SendTransaction(RawTransaction request)
        {
            FaceRpcRequest<RawTransaction> rpcRequest =
                new FaceRpcRequest<RawTransaction>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.eth_sendTransaction, request);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Call(RawTransaction request)
        {
            FaceRpcRequest<object> rpcRequest = new FaceRpcRequest<object>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.eth_call, request, "latest");
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Sign(string message)
        {
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), FaceRpcMethod.personal_sign,
                string.Format($"0x{string.Join("", message.Select(c => ((int)c).ToString("X2")))}"));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }
    }
}