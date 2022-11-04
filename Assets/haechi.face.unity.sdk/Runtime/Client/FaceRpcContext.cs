using Nethereum.JsonRpc.Client.RpcMessages;

namespace haechi.face.unity.sdk.Runtime.Client
{
    public class FaceRpcContext
    {
        public FaceRpcResponse Response { get; private set; }
        public WebviewRpcRequest Request { get; private set; }

        private FaceRpcContext()
        {
        } 

        public FaceRpcContext(FaceRpcResponse response)
        {
            this.Response = response;
        }
        
        public FaceRpcContext(WebviewRpcRequest request)
        {
            this.Request = request;
        }

        public bool WebviewRequest()
        {
            return this.Request != null;
        }
    }
}