using System;
using TMPro;
using UnityEngine;

public class UIDataField : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private TextMeshProUGUI _dataText;

    [SerializeField] private UIButton _copyButton;

    private void OnEnable()
    {
        this._copyButton.OnClickEvent += this.CopyText;
    }

    private void OnDisable()
    {
        this._copyButton.OnClickEvent -= this.CopyText;
    }

    private void CopyText()
    {
        GUIUtility.systemCopyBuffer = this._dataText.text;
    }
    
    public void SetData(string value)
    {
        this._dataText.text = value;
    }
}