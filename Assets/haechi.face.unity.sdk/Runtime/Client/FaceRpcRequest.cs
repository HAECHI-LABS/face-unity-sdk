using System;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace haechi.face.unity.sdk.Runtime.Client
{
    [Serializable]
    public class FaceRpcRequest<T>
    {
        [SerializeField] internal int id;
        [SerializeField] [CanBeNull] internal string jsonrpc = "2.0";
        [SerializeField] internal string method;
        [SerializeField] [CanBeNull] internal string userId;
        [SerializeField] [CanBeNull] internal string blockchain;
        [SerializeField] internal T[] @params;

        public FaceRpcRequest(FaceRpcMethod method, params T[] parameters)
        {
            this.id = Random.Range(1, 100000);
            this.method = Enum.GetName(typeof(FaceRpcMethod), method);
            this.@params = parameters;
        }
    }
}