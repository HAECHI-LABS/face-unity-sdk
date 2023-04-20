using System;
using TMPro;
using UnityEngine;

public class UISectionSignMessage : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField _messageInputField;
    [SerializeField] private UIButton _signMessageButton;
    
    [Header("Listening on")]
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private VoidEventChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;
    
    [Header("Broadcast to")] 
    [SerializeField] private StringEventChannelSO _signMessage;

    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._signMessageButton.OnClickEvent += this.SignMessage;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._signMessageButton.OnClickEvent -= this.SignMessage;
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
    }
    
    private void Initialize()
    {
        if (!this._appState.LoggedIn())
        {
            this._signMessageButton.UI.interactable = false;
            return;
        }
        
        this._signMessageButton.UI.interactable = true;
    }

    private void SignMessage()
    {
        this._signMessage.RaiseEvent(this._messageInputField.text);
    }
}