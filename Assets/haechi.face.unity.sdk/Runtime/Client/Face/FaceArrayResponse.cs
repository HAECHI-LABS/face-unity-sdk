using System;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    public class FaceArrayResponse
    {
        [SerializeField] internal string[] response;

        public FaceArrayResponse(string[] response)
        {
            this.response = response;
        }
    }
}