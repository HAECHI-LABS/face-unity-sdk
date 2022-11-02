namespace haechi.face.unity.sdk.Runtime.Exception
{
    public struct ErrorCode
    {
        public string Value;
        public string Message;
    }

    public static class ErrorCodes
    {
        public static readonly ErrorCode INVALID_WEBVIEW_MESSAGE = new ErrorCode { Value = "U0001", Message = "Invalid message comes from webview" };
        public static readonly ErrorCode INVALLID_RPC_RESPONSE = new ErrorCode { Value = "U0002", Message = "Invalid rpc response" };
        public static readonly ErrorCode INVALLID_RPC_METHOD = new ErrorCode { Value = "U0003", Message = "Invalid rpc method" };
        public static readonly ErrorCode ALREADY_INITIALIZED = new ErrorCode { Value = "U0004", Message = "Face is already initialized" };
        public static readonly ErrorCode INVALLID_RPC_REQUEST = new ErrorCode { Value = "U0005", Message = "Invalid rpc request" };
        public static readonly ErrorCode SERVER_RESPONSE_ERROR = new ErrorCode { Value = "U0006", Message = "Face server returned an error" };
    }

    public class FaceException : System.Exception
    {
        public readonly ErrorCode errorCode;

        public FaceException(ErrorCode errorCode) : base(errorCode.Message)
        {
            this.errorCode = errorCode;
        }

        public FaceException(ErrorCode errorCode, System.Exception exception) : base(errorCode.Message, exception)
        {
            this.errorCode = errorCode;
        }

        public FaceException(ErrorCode errorCode, string message) : base(message)
        {
            this.errorCode = errorCode;
        }

        public FaceException(ErrorCode errorCode, string message, System.Exception exception) : base(message, exception)
        {
            this.errorCode = errorCode;
        }
    }
}