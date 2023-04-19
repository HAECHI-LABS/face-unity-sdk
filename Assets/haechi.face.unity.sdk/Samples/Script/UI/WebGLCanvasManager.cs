using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGLCanvasManager : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;

#if !UNITY_WEBGL
    private void Start()
    {
        this._canvas.gameObject.SetActive(false);
    }
#endif
    
#if UNITY_WEBGL
    private void Start()
    {
        this._canvas.gameObject.SetActive(true);
    }
#endif
}

