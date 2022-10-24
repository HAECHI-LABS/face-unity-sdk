namespace haechi.face.unity.sdk.Runtime.Exception
{
    public class InvalidWebviewMessageException : FaceException
    {
        public InvalidWebviewMessageException() : base(ErrorCodes.INVALID_WEBVIEW_MESSAGE)
        {
        }

        public InvalidWebviewMessageException(string message) : base(ErrorCodes.INVALID_WEBVIEW_MESSAGE, message)
        {
        }
    }

    public class InvalidRpcResponse : FaceException
    {
        public InvalidRpcResponse(System.Exception exception) : base(ErrorCodes.INVALLID_RPC_RESPONSE, exception)
        {
        }
    }
}