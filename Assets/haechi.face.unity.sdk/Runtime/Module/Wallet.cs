using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.Face;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Utils;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class Wallet : IWallet
    {
        private readonly FaceRpcProvider _provider;
        private readonly FaceClient _client;
        private string _requestId;

        internal Wallet(FaceRpcProvider provider)
        {
            this._provider = provider;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
        }

        /// <summary>
        /// Sign up(if new user) or log in function. Need to initialize face with environment, blockchain and api key first.&#10;
        /// You can choose three options, Google, Facebook, and Apple login.
        /// </summary>
        /// <returns>
        /// <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.Face.FaceLoginResponse.html">FaceLoginResponse</a>. Unique user ID using on Face server and wallet address.
        /// </returns>
        /// <exception cref="FaceException">Throws FaceExceptioin when address verification fails.</exception>
        public async Task<FaceLoginResponse> LoginWithCredential() 
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
        /// Check if session logged in.
        /// </summary>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>. Result is boolean value.</returns>
        public async Task<FaceRpcResponse> IsLoggedIn()
        {
            FaceRpcRequest<string> request = new FaceRpcRequest<string>(FaceSettings.Instance.Blockchain(),
                FaceRpcMethod.face_loggedIn);
            return await this._provider.SendFaceRpcAsync(request);
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
    }
}