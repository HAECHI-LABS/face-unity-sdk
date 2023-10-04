using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using haechi.face.unity.sdk.Runtime.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public interface IAuth
    {
        Task<FaceLoginResponse> Login([AllowNull] List<LoginProviderType> providers);
        Task<FaceLoginResponse> DirectSocialLogin(string provider);
        Task<FaceLoginResponse> DirectSocialLogin(LoginProviderType provider);
        Task<FaceRpcResponse> Logout();
    }
    
    public class Auth : IAuth
    {
        private readonly FaceRpcProvider _provider;
        internal FaceLoginResponse CurrentUser;
        
        internal Auth(FaceRpcProvider provider)
        {
            this._provider = provider;
        }
        
        /// <summary>
        /// Sign-up(if new user) or login function. Need to initialize face with environment, blockchain and api key first.&#10;
        /// You can pass all options contained in <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Type.LoginProviderType.html">LoginProviderType</a>.
        /// Or you can just pass empty or null values.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="AddressVerificationFailedException">Throws AddressVerificationFailedException when address verification fails.</exception>
        public async Task<FaceLoginResponse> Login([AllowNull] List<LoginProviderType> providers)
        {
            string[] providerHosts = providers?.ConvertAll(provider => provider.HostValue()).ToArray();
            return await this._login(FaceRpcMethod.face_logInSignUp, providerHosts);
        }

        /// <summary>
        /// Directly sign-up(if new user) or login using social login. Need to initialize face with environment, blockchain and api key first.&#10;
        /// Pass the desired <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Type.LoginProviderType.html">login provider</a> as host value to parameter.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="AddressVerificationFailedException">Throws AddressVerificationFailedException when address verification fails.</exception>
        public async Task<FaceLoginResponse> DirectSocialLogin(string provider)
        {
            return await this._login(FaceRpcMethod.face_directSocialLogin, provider);
        }
        
        /// <summary>
        /// Directly sign-up(if new user) or login using social login. Need to initialize face with environment, blockchain and api key first.&#10;
        /// Pass the desired <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Type.LoginProviderType.html">login provider</a> to parameter.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="AddressVerificationFailedException">Throws AddressVerificationFailedException when address verification fails.</exception>
        public async Task<FaceLoginResponse> DirectSocialLogin(LoginProviderType provider)
        {
            return await this._login(FaceRpcMethod.face_directSocialLogin, provider.HostValue());
        }
        
        /// <summary>
        /// login with id token. Need to initialize face with environment, blockchain and api key first.&#10;
        /// Pass the desired <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Type.LoginProviderType.html">login provider</a> to parameter.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="AddressVerificationFailedException">Throws AddressVerificationFailedException when address verification fails.</exception>
        public async Task<FaceLoginResponse> LoginWithIdToken(FaceLoginIdTokenRequest loginIdTokenRequest)
        {
            return await this._loginWithIdToken(FaceRpcMethod.face_loginWithIdToken, loginIdTokenRequest);
        }

        /// <summary>
        /// Check is logged in
        /// </summary>
        /// <returns>Boolean value. If logged in, returns true.</returns>
        public bool IsLoggedIn()
        {
            return this.CurrentUser != null;
        }

        private async Task<FaceLoginResponse> _login(FaceRpcMethod method, params string[] parameterList)
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(), method, parameterList);
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(request);

            FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();
            FaceLoginResponse.Wallet wallet = faceLoginResponse.wallet;
            
            // TODO: Temporally disable verification
            // if (!RSASignatureVerifier.Verify(wallet.Address, wallet.SignedAddress, FaceSettings.Instance.ApiKey()))
            // {
            //     throw new AddressVerificationFailedException();
            // }

            this.CurrentUser = faceLoginResponse;
            return this.CurrentUser;
        }
        
        private async Task<FaceLoginResponse> _loginWithIdToken(FaceRpcMethod method, params FaceLoginIdTokenRequest[] parameterList)
        {
            FaceRpcRequest<FaceLoginIdTokenRequest> request = new FaceRpcRequest<FaceLoginIdTokenRequest>(FaceSettings.Instance.Blockchain(), method, parameterList);
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(request);

            FaceLoginResponse faceLoginResponse = response.CastResult<FaceLoginResponse>();
            FaceLoginResponse.Wallet wallet = faceLoginResponse.wallet;
            
            // TODO: Temporally disable verification
            // if (!RSASignatureVerifier.Verify(wallet.Address, wallet.SignedAddress, FaceSettings.Instance.ApiKey()))
            // {
            //     throw new AddressVerificationFailedException();
            // }
            
            this.CurrentUser = faceLoginResponse;
            return this.CurrentUser;
        }

        /// <summary>
        /// Log out Face Wallet.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is boolean value.</returns>
        public async Task<FaceRpcResponse> Logout()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_logOut);
            FaceRpcResponse response = await this._provider.SendFaceRpcAsync(request);
            this.CurrentUser = null;
            return response;
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
            return this._auth.Login(null);
        }
        
        public Task<FaceLoginResponse> DirectSocialLogin(string provider)
        {
            return this._auth.DirectSocialLogin(provider);
        }
        
        public Task<FaceLoginResponse> DirectSocialLogin(LoginProviderType provider)
        {
            return this._auth.DirectSocialLogin(provider);
        }
        
        public Task<FaceRpcResponse> Logout()
        {
            return this._auth.Logout();
        }
    }
}