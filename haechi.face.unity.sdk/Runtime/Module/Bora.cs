using System;
using System.Net.Http;
using System.Threading.Tasks;
using haechi.face.unity.sdk.Runtime.Client;
using haechi.face.unity.sdk.Runtime.Client.BoraPortal;
using haechi.face.unity.sdk.Runtime.Exception;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Module
{
    public class Bora
    {
        private readonly FaceRpcProvider _provider;
        private readonly FaceClient _client;
        private readonly Auth _auth;
        
        internal Bora(FaceRpcProvider provider, Auth auth)
        {
            this._provider = provider;
            this._client = new FaceClient(new Uri(FaceSettings.Instance.ServerHostURL()), new HttpClient());
            this._auth = auth;
        }

        /// <summary>
        /// Connect to BoraPortal
        /// </summary>
        /// <param name="request"><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.BoraPortal.BoraPortalConnectRequest.html">BoraPortalConnectRequest</a></param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.FaceRpcResponse.html">FaceRpcResponse</a>.
        /// Result is <a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.BoraPortal.BoraPortalConnectStatusResponse.html">BoraPortalConnectStatusResponse</a>.
        /// </returns>
        public async Task<FaceRpcResponse> Connect(BoraPortalConnectRequest request)
        {
            if (!Blockchain.BORA.Equals(FaceSettings.Instance.Blockchain()))
            {
                throw new BoraConnectInvalidBlockchainException();
            }
            FaceRpcRequest<BoraPortalConnectRequest> rpcRequest = new FaceRpcRequest<BoraPortalConnectRequest>(FaceSettings.Instance.Network(), FaceRpcMethod.bora_connect, request);
            return await this._provider.SendFaceRpcAsync(rpcRequest);
        }
        
        /// <summary>
        /// Check if user is connected to Bapp.
        /// </summary>
        /// <param name="bappUsn">Unique user id.</param>
        /// <returns><a href="https://unity.api-reference.facewallet.xyz/api/haechi.face.unity.sdk.Runtime.Client.BoraPortal.BoraPortalConnectStatusResponse.html">BoraPortalConnectStatusResponse</a>.
        /// </returns>
        public async Task<BoraPortalConnectStatusResponse> IsConnected(string bappUsn)
        {
            Blockchain blockchain = FaceSettings.Instance.Blockchain();
            if (!Blockchain.BORA.Equals(blockchain))
            {
                throw new BoraConnectInvalidBlockchainException();
            }
            
#if UNITY_WEBGL
            Task<BoraPortalConnectStatusResponse> task = this._provider.WebRequest.SendHttpGetRequest<BoraPortalConnectStatusResponse>(
                $"/v1/bora/portal/get-connect-status-without-session?userId={this._auth.CurrentUser.faceUserId}&bappUsn={bappUsn}");
#else
            Task<BoraPortalConnectStatusResponse> task = this._client.SendHttpGetRequest<BoraPortalConnectStatusResponse>(
                $"/v1/bora/portal/get-connect-status-without-session?userId={this._auth.CurrentUser.faceUserId}&bappUsn={bappUsn}");
#endif

            try
            {
                return await task;
            }
            catch (HttpRequestException e)
            {
                throw new FaceServerException(e);
            }
        }
    }
}