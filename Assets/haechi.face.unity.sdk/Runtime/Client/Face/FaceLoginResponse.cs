using System;
using UnityEngine;

namespace haechi.face.unity.sdk.Runtime.Client.Face
{
    [Serializable]
    public class FaceLoginResponse
    {
        [SerializeField] internal string faceUserId;

        public FaceLoginResponse(string faceUserId)
        {
            this.faceUserId = faceUserId;
        }
    }
}