using System;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    public class FaceGetBalanceResponse
    {
        [SerializeField] internal string address;
        [SerializeField] internal string balance;

        public FaceGetBalanceResponse(string address, string balance)
        {
            this.address = address;
            this.balance = balance;
        }
    }
}