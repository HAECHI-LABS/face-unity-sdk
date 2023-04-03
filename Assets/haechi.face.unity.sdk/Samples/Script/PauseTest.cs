using System;
using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class PauseTest : MonoBehaviour
    {
        public void OnApplicationFocus(bool hasFocus)
        {
            Debug.Log($"OnApplicationFocus: {hasFocus}\n");
        }

        public void OnApplicationPause(bool pauseStatus)
        {
            Debug.Log($"OnApplicationPause: {pauseStatus}\n");
        }
    }
}