using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DeviceScreenType
{
    None,
    Portrait,
    Landscape,
}

[Serializable]
public class DeviceScreen
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private DeviceScreenType _type;

    public Canvas Canvas => this._canvas;
    public DeviceScreenType Type => this._type;
}

public class DeviceCanvasManager : MonoBehaviour
{
    [SerializeField] private float _checkDelaySeconds;
    
    [SerializeField] private List<DeviceScreen> _screens;
    
    private Coroutine _detectDeviceOrientationRoutine;

#if UNITY_WEBGL
    private void Start()
    {
        this.DeactivateAll();
    }
#endif

#if !UNITY_WEBGL
    private void Start()
    {
#if UNITY_EDITOR
        if (this.IsAllCanvasInactive())
        {
            return;
        }
#endif
        this.InitializeScreens();
    }

    private void OnDestroy()
    {
        if (this._detectDeviceOrientationRoutine != null)
        {
            this.StopCoroutine(this._detectDeviceOrientationRoutine);
        }
    }

    private void InitializeScreens()
    {
        this.DeactivateAll();
        
        this.ActivateBy(this.GetCurrentScreenType());
        this._detectDeviceOrientationRoutine = this.StartCoroutine(this.DetectDeviceOrientationChanges());
    }

    private IEnumerator DetectDeviceOrientationChanges()
    {
        while (true)
        {
            if (this.IsPortrait())
            {
                this.ActivateBy(DeviceScreenType.Portrait);
            }
            else
            {
                this.ActivateBy(DeviceScreenType.Landscape);
            }
            
            yield return new WaitForSeconds(this._checkDelaySeconds);
        }
    }


    private bool IsPortrait()
    {
        DeviceOrientation deviceOrientation = Input.deviceOrientation;
        bool isPortraitFromDevice  = DeviceOrientation.Portrait == deviceOrientation
               || Screen.orientation == ScreenOrientation.Portrait
               || Screen.orientation == ScreenOrientation.PortraitUpsideDown;
        bool isPortraitFromEditor = Screen.height > Screen.width;
        if (Application.isEditor)
        {
            return isPortraitFromEditor;
        }
        else
        {
            return isPortraitFromDevice;
        }
    }

    private DeviceScreenType GetCurrentScreenType()
    {
        return this.IsPortrait() ? DeviceScreenType.Portrait : DeviceScreenType.Landscape;
    }

    private void ActivateBy(DeviceScreenType deviceScreenType)
    {
        this._screens.ForEach(s =>
        {
            if (s.Type.Equals(deviceScreenType))
            {
                s.Canvas.gameObject.SetActive(true);
                return;
            }
            s.Canvas.gameObject.SetActive(false);
        });
    }
#if UNITY_EDITOR
    private bool IsAllCanvasInactive()
    {
        return this._screens
            .ConvertAll(s => !s.Canvas.gameObject.activeSelf)
            .Aggregate(true, (result, isInactive) => result && isInactive);
    }
#endif
#endif
    
    private void DeactivateAll()
    {
        this._screens.ForEach(s => s.Canvas.gameObject.SetActive(false));
    }
}