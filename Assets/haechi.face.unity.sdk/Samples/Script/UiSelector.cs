using UnityEngine;

namespace haechi.face.unity.sdk.Samples.Script
{
    public class UiSelector : MonoBehaviour
    {
        public GameObject portraitUI, landscapeUI, webGlUi;

        private void Update()
        {
#if UNITY_WEBGL
            this.portraitUI.SetActive(false);
            this.landscapeUI.SetActive(false);
            this.webGlUi.SetActive(true);
#else
            if (_isPortrait())
            {
                this.landscapeUI.SetActive(false);
                this.portraitUI.SetActive(true);
                return;
            }

            this.portraitUI.SetActive(false);
            this.landscapeUI.SetActive(true);
#endif
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