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
    
    public class InvalidRpcRequestException : FaceException {
        public InvalidRpcRequestException(string message) : base(ErrorCodes.INVALLID_RPC_RESPONSE, message) {}
    }

    public class InvalidRpcResponseException : FaceException
    {
        public InvalidRpcResponseException(System.Exception exception) : base(ErrorCodes.INVALLID_RPC_RESPONSE, exception) {}
    }
    
    public class InvalidRpcMethodException : FaceException
    {
        public InvalidRpcMethodException() : base(ErrorCodes.INVALLID_RPC_METHOD) {}
    }
}