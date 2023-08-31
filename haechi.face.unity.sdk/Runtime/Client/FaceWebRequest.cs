using System.Threading.Tasks;
using Nethereum.JsonRpc.Client.RpcMessages;
using Newtonsoft.Json;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceWebRequest
    {
        private MonoBehaviour _face;

        public FaceWebRequest(MonoBehaviour face)
        {
            this._face = face;
        }

        public async Task<TR> SendHttpGetRequest<TR>(string uri)
        {
            UnityWebRequestService unityWebRequestService = new UnityWebRequestService(this._face);
            unityWebRequestService.Get($"{FaceSettings.Instance.ServerHostURL()}{uri}");
            return await unityWebRequestService.GetResult<TR>();
        }
        
        public async Task<FaceRpcResponse> SendRpcRequest(string uri, RpcRequestMessage request)
        {
            UnityWebRequestService unityWebRequestService = new UnityWebRequestService(this._face);
            unityWebRequestService.Post($"{FaceSettings.Instance.ServerHostURL()}{uri}", JsonConvert.SerializeObject(request));
            return await unityWebRequestService.GetResult<FaceRpcResponse>();
        }
    }
}