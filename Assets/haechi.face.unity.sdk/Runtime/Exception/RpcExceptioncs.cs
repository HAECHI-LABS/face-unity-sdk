using System.Net.Http;
using Newtonsoft.Json;

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

    public class WebviewClosedException : FaceException
    {
        public WebviewClosedException() : base(ErrorCodes.WEBVIEW_CLOSED) {}
    }

    public class FaceServerException : FaceException
    {
        public FaceServerException(FaceServerError error) : base(ErrorCodes.SERVER_RESPONSE_ERROR, error.ToString())
        {
        }
    }
    
    /// <summary>
    /// FaceServerError is a data for server error
    /// </summary>
    [JsonObject]
    public class FaceServerError
    {
        /// <summary>
        /// Timestamp for when the server handles the error
        /// </summary>
        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp;
        
        /// <summary>
        /// Path for the request
        /// </summary>
        [JsonProperty(PropertyName = "path")]
        public string Path;
        
        /// <summary>
        /// Error is a message for the cause of error
        /// </summary>
        [JsonProperty(PropertyName = "error")]
        public string Error;
        
        /// <summary>
        /// RequestId is a identifier for the request
        /// </summary>
        [JsonProperty(PropertyName = "requestId")]
        public string RequestId;
        
        /// <summary>
        /// Message is a message for the cause of error
        /// </summary>
        [JsonProperty(PropertyName = "message")]
        public string Message;
        
        /// <summary>
        /// Code about error cause
        /// </summary>
        [JsonProperty(PropertyName = "code")]
        public string Code;
        
        /// <summary>
        /// Status is a HTTP status code
        /// </summary>
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
}