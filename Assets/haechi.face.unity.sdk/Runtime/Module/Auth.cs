using System;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public interface IAuth
    {
        Task<FaceLoginResponse> Login();
        Task<FaceRpcResponse> Logout();
    }
    
    public class Auth : IAuth
    {
        private readonly FaceRpcProvider _provider;
        
        internal Auth(FaceRpcProvider provider)
        {
            this._provider = provider;
        }
        
        /// <summary>
        /// Sign up(if new user) or log in function. Need to initialize face with environment, blockchain and api key first.&#10;
        /// You can choose three options, Google, Facebook, and Apple login.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="FaceException">Throws FaceExceptioin when address verification fails.</exception>
        public async Task<FaceLoginResponse> Login() 
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), 
                FaceRpcMethod.face_logInSignUp);
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(request);
            FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();

            FaceLoginResponse.Wallet wallet = faceLoginResponse.wallet;
            
            if (!RSASignatureVerifier.Verify(wallet.Address, wallet.SignedAddress, FaceSettings.Instance.ApiKey()))
            {
                throw new FaceException(ErrorCodes.ADDRESS_VERIFICATION_FAILED);
            }
            
            return faceLoginResponse;
        }

        /// <summary>
        /// Log out Face Wallet.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is boolean value.</returns>
        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_logOut);
            return await this._provider.SendFaceRpcAsync(request);
        }
    }

    public class AuthProxy
    {
        private IAuth _auth;

        public void Register(IAuth auth)
        {
            this._auth = auth;
        }
        
        public Task<FaceLoginResponse> Login()
        {
            return this._auth.Login();
        }
        
        public Task<FaceRpcResponse> Logout()
        {
            return this._auth.Logout();
        }
    }
}