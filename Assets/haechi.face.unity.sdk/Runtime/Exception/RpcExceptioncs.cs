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
}