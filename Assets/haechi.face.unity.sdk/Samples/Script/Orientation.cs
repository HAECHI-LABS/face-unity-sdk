using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class Orientation : MonoBehaviour
    {
        public GameObject portraitUI, landscapeUI;

        private void Update()
        {
            if (_isPortrait())
            {
                this.landscapeUI.SetActive(false);
                this.portraitUI.SetActive(true);
                return;
            }

            this.portraitUI.SetActive(false);
            this.landscapeUI.SetActive(true);
        }

        private static bool _isPortrait()
        {
            DeviceOrientation deviceOrientation = Input.deviceOrientation;
            return DeviceOrientation.Portrait == deviceOrientation
                   || Screen.orientation == ScreenOrientation.Portrait
                   || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        }
    }
}