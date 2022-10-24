using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Type;

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
                FaceRpcRequest<FaceEnvironments>.Of(FaceRpcMethod.wallet_initialize, env);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> SwitchNetwork(string network)
        {
            FaceRpcRequest<int> rpcRequest = FaceRpcRequest<int>.Of(FaceRpcMethod.wallet_switchEthereumChain,
                NetworkResolver.GetChainId(network));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public Task<FaceRpcResponse> LoginWithCredential() // Action<FaceRpcResponse> -> Action<FaceLoginResponse> 
        {
            FaceRpcRequest<string> request = FaceRpcRequest<string>.Of(FaceRpcMethod.face_logInSignUp);
            return this._client.SendFaceRpcAsync<string, FaceLoginResponse>(request);
        }

        public async Task<FaceRpcResponse> IsLoggedIn()
        {
            FaceRpcRequest<string> request = FaceRpcRequest<string>.Of(FaceRpcMethod.face_loggedIn);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = FaceRpcRequest<string>.Of(FaceRpcMethod.face_logOut);
            return await this._client.SendFaceRpcAsync(request);
        }

        public async Task<FaceRpcResponse> GetAddresses()
        {
            return await this._client.SendFaceRpcAsync<string, FaceArrayResponse>(
                FaceRpcRequest<string>.Of(FaceRpcMethod.face_accounts));
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
                FaceRpcRequest<RawTransaction>.Of(FaceRpcMethod.eth_sendTransaction, request);
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Call(RawTransaction request)
        {
            FaceRpcRequest<object> rpcRequest = FaceRpcRequest<object>.Of(FaceRpcMethod.eth_call, request, "latest");
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }

        public async Task<FaceRpcResponse> Sign(string message)
        {
            FaceRpcRequest<string> rpcRequest = FaceRpcRequest<string>.Of(FaceRpcMethod.personal_sign,
                string.Format($"0x{string.Join("", message.Select(c => ((int)c).ToString("X2")))}"));
            return await this._client.SendFaceRpcAsync(rpcRequest);
        }
    }
}