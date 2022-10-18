using System;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    public class FaceSendTransactionResponse
    {
        [SerializeField] internal Transaction tx;
    }

    [Serializable]
    public class Transaction
    {
        [SerializeField] internal string hash;
        [SerializeField] [CanBeNull] internal string blockHash;
        [SerializeField] internal string from;
        [SerializeField] [CanBeNull] internal string raw;
        [SerializeField] internal BigInteger? blockNumber;
        [SerializeField] internal BigInteger confirmations;
        [SerializeField] internal BigInteger? timestamp;
    }
}