using Newtonsoft.Json;

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
        public static readonly ErrorCode INVALLID_RPC_REQUEST = new ErrorCode { Value = "U0002", Message = "Invalid rpc request" };
        public static readonly ErrorCode INVALLID_RPC_RESPONSE = new ErrorCode { Value = "U0003", Message = "Invalid rpc response" };
        public static readonly ErrorCode INVALLID_RPC_METHOD = new ErrorCode { Value = "U0004", Message = "Invalid rpc method" };
        public static readonly ErrorCode ALREADY_INITIALIZED = new ErrorCode { Value = "U0005", Message = "Face is already initialized" };
        public static readonly ErrorCode NOT_INITIALIZED = new ErrorCode { Value = "U0006", Message = "Face is not initialized yet" };
        public static readonly ErrorCode SERVER_RESPONSE_ERROR = new ErrorCode { Value = "U0007", Message = "Face server returned an error" };
        public static readonly ErrorCode ADDRESS_VERIFICATION_FAILED = new ErrorCode { Value = "U0008", Message = "Failed to verify address" };
        public static readonly ErrorCode UNAUTHORIZED = new ErrorCode { Value = "U0009", Message = "Not logged in yet" };
        public static readonly ErrorCode WEBVIEW_CLOSED = new ErrorCode { Value = "U0010", Message = "Webview is closed" };
        public static readonly ErrorCode FAILED_TO_SWITCH_NETWORK = new ErrorCode { Value = "U0011", Message = "Failed to switch network." };
        public static readonly ErrorCode BORA_CONNECT_BLOCKCHAIN_INVALID = new ErrorCode { Value = "U0012", Message = "Only BORA network can use this method." };
    }
    
    public class InvalidWebviewMessageException : FaceException
    {
        public InvalidWebviewMessageException() : base(ErrorCodes.INVALID_WEBVIEW_MESSAGE)
        {
        }
    }
    
    public class InvalidRpcRequestException : FaceException {
        public InvalidRpcRequestException(string message) : base(ErrorCodes.INVALLID_RPC_REQUEST, message) {}
    }

    public class InvalidRpcResponseException : FaceException
    {
        public InvalidRpcResponseException(System.Exception exception) : base(ErrorCodes.INVALLID_RPC_RESPONSE, exception) {}
    }
    
    public class InvalidRpcMethodException : FaceException
    {
        public InvalidRpcMethodException() : base(ErrorCodes.INVALLID_RPC_METHOD) {}
    }
    
    public class AlreadyInitializedException : FaceException
    {
        public AlreadyInitializedException() : base(ErrorCodes.ALREADY_INITIALIZED) {}
    }
    
    public class NotInitializedException : FaceException
    {
        public NotInitializedException() : base(ErrorCodes.NOT_INITIALIZED) {}
    }
    
    public class FaceServerException : FaceException
    {
        public FaceServerException(System.Exception error) : base(ErrorCodes.SERVER_RESPONSE_ERROR, error.ToString())
        {
        }
        
        public FaceServerException(FaceServerError error) : base(ErrorCodes.SERVER_RESPONSE_ERROR, error.ToString())
        {
        }
    }
    
    public class AddressVerificationFailedException : FaceException
    {
        public AddressVerificationFailedException() : base(ErrorCodes.ADDRESS_VERIFICATION_FAILED) {}
    }

    public class UnauthorizedException : FaceException
    {
        public UnauthorizedException() : base(ErrorCodes.UNAUTHORIZED) {}
    }
    
    public class WebviewClosedException : FaceException
    {
        public WebviewClosedException() : base(ErrorCodes.WEBVIEW_CLOSED) {}
    }
    
    public class SwitchNetworkFailedException : FaceException
    {
        public SwitchNetworkFailedException() : base(ErrorCodes.FAILED_TO_SWITCH_NETWORK) {}
    }
    
    public class BoraConnectInvalidBlockchainException : FaceException
    {
        public BoraConnectInvalidBlockchainException() : base(ErrorCodes.BORA_CONNECT_BLOCKCHAIN_INVALID) {}
    }

    /// <value>
    /// FaceServerError is a data for server error.
    /// </value>
    [JsonObject]
    public class FaceServerError
    {
        /// <value>
        /// Timestamp for when the server handles the error.
        /// </value>
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp;
        
        /// <value>
        /// Path for the request.
        /// </value>
        [JsonProperty(PropertyName = "path")]
        public string Path;
        
        /// <value>
        /// Error is a message for the cause of error.
        /// </value>
        [JsonProperty(PropertyName = "error")]
        public string Error;
        
        /// <value>
        /// RequestId is a identifier for the request.
        /// </value>
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId;
        
        /// <value>
        /// Message is a message for the cause of error.
        /// </value>
        [JsonProperty(PropertyName = "message")]
        public string Message;
        
        /// <value>
        /// Code about error cause.
        /// </value>
        [JsonProperty(PropertyName = "code")]
        public string Code;
        
        /// <value>
        /// Status is a HTTP status code.
        /// </value>
        [JsonProperty(PropertyName = "status")]
        public int Status;

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.None, 
                new JsonSerializerSettings { 
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
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