using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;

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
                new FaceRpcRequest<FaceEnvironments>(FaceRpcMethod.wallet_initialize, env);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> SwitchNetwork(string network)
        {
            FaceRpcRequest<int> rpcRequest = new FaceRpcRequest<int>(FaceRpcMethod.wallet_switchEthereumChain,
                NetworkResolver.GetChainId(network));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public Task<FaceRpcResponse> LoginWithCredential() // Action<FaceRpcResponse> -> Action<FaceLoginResponse> 
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceRpcMethod.face_logInSignUp);
            return this._client.SendFaceRpcAsync<string, FaceLoginResponse>(request);
        }

        public async Task<FaceRpcResponse> IsLoggedIn()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceRpcMethod.face_loggedIn);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceRpcMethod.face_logOut);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> GetAddresses()
        {
            return await this._client.SendFaceRpcAsync<string, FaceArrayResponse>(
                new FaceRpcRequest<string>(FaceRpcMethod.face_accounts));
        }

        public async Task<FaceRpcResponse> GetBalance(string account = null)
        {
            // string address = account ?? ((FaceArrayResponse)(await this.GetAddresses()).result).response[0];
            // return await this._client.SendFaceRpcAsync(new FaceRpcRequest<string>(FaceRpcMethod.eth_getBalance, address,
            //     "latest"));
            return null;
        }

        public async Task<FaceRpcResponse> SendTransaction(RawTransaction request)
        {
            FaceRpcRequest<RawTransaction> rpcRequest =
                new FaceRpcRequest<RawTransaction>(FaceRpcMethod.eth_sendTransaction, request);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Call(RawTransaction request)
        {
            FaceRpcRequest<object> rpcRequest = new FaceRpcRequest<object>(FaceRpcMethod.eth_call, request, "latest");
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Sign(string message)
        {
            FaceRpcRequest<string> rpcRequest = new FaceRpcRequest<string>(FaceRpcMethod.personal_sign,
                string.Format($"0x{string.Join("", message.Select(c => ((int)c).ToString("X2")))}"));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }
        
        
    }
}