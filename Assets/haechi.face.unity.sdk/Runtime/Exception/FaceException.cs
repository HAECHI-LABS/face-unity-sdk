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
        public static readonly ErrorCode NOT_INITIALIZED = new ErrorCode { Value = "U0005", Message = "Face is not initialized yet" };
        public static readonly ErrorCode INVALLID_RPC_REQUEST = new ErrorCode { Value = "U0006", Message = "Invalid rpc request" };
        public static readonly ErrorCode SERVER_RESPONSE_ERROR = new ErrorCode { Value = "U0007", Message = "Face server returned an error" };
        public static readonly ErrorCode ADDRESS_VERIFICATION_FAILED = new ErrorCode { Value = "U0008", Message = "Failed to verify address" };
        public static readonly ErrorCode UNAUTHORIZED = new ErrorCode { Value = "U0009", Message = "Not logged in yet" };
        public static readonly ErrorCode WEBVIEW_CLOSED = new ErrorCode { Value = "U0010", Message = "Webview is closed" };
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