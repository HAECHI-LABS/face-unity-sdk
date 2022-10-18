using System;
using System.Collections.Generic;
using Nethereum.JsonRpc.Client.RpcMessages;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    public class FaceRpcResponse
    {
        [SerializeField] internal string id;
        [SerializeField] internal string method;
        [SerializeField] internal FaceRpcError error;
        [SerializeField] internal object result;

        private FaceRpcResponse()
        {
        }

        public FaceRpcResponse(string id, string method, object result, RpcError error)
        {
            this.id = id;
            this.method = method;
            this.result = result;
            this.error = error != null ? new FaceRpcError(error.Code, error.Message, error.Data.ToString()) : null;
        }

        public static FaceRpcResponse OfDictionary(IDictionary<string, string> dictionary)
        {
            FaceRpcResponse response = new FaceRpcResponse();
            foreach (KeyValuePair<string, string> keyValuePair in dictionary)
            {
                string key = keyValuePair.Key;
                string value = keyValuePair.Value;

                switch (key)
                {
                    case "id":
                        response.id = value;
                        break;
                    case "method":
                        response.method = value;
                        break;
                    case "result":
                        response.result = value;
                        break;
                    case "error":
                        response.error = JsonUtility.FromJson<FaceRpcError>(value);
                        break;
                }
            }

            if (response.id == null || response.method == null || (response.result == null && response.error == null))
            {
                throw new ArgumentException("Response should include id, method, and result, or error");
            }

            return response;
        }
    }

    [Serializable]
    public class FaceRpcError
    {
        [SerializeField] internal int code;
        [SerializeField] internal string message;
        [SerializeField] internal string data;

        public FaceRpcError(int code, string message, string data)
        {
            this.code = code;
            this.message = message;
            this.data = data;
        }
    }
}