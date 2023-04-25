using System;
using TMPro;
using UnityEngine;

public class UISectionPlatformCoinTransaction : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;
    [SerializeField] private float _defaultSendAmount;
    
    [Header("UI References")] 
    [SerializeField] private TMP_InputField _amountInputField;
    [SerializeField] private TMP_InputField _receiverAddressInputField;
    [SerializeField] private UIButton _sendButton;
    
    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private VoidEventChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;

    [Header("Broadcast to")] 
    [SerializeField] private FTTransactionDataChannelSO _sendPlatformCoin;

    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._sendButton.OnClickEvent += this.Send;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._sendButton.OnClickEvent -= this.Send;
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
    }

    private void Initialize()
    {
        this._amountInputField.text = this._defaultSendAmount.ToString();
        
        if (!this._appState.LoggedIn())
        {
            this._sendButton.UI.interactable = false;
            
            return;
        }
        this._receiverAddressInputField.text = this._appState.GetLoginData().UserAddress;
        this._sendButton.UI.interactable = true;
    }

    private void Send()
    {
        this._sendPlatformCoin.RaiseEvent(new FTTransactionData
        {
            Amount = this._amountInputField.text,
            ReceiverAddress = this._receiverAddressInputField.text,
        });
    }
}