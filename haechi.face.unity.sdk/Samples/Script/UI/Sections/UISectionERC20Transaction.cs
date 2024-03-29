using System;
using TMPro;
using UnityEngine;

public class UISectionERC20Transaction : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;
    [SerializeField] private float _defaultSendAmount;
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField _contractAddressInputField;
    [SerializeField] private TMP_InputField _amountInputField;
    [SerializeField] private TMP_InputField _receiverAddressInputField;
    [SerializeField] private UIButton _sendButton;
    
    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private LoginDataChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;

    [Header("Broadcast to")] 
    [SerializeField] private FTTransactionDataChannelSO _sendERC20;
    
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

    private void Initialize(LoginData loginData)
    {
        this.Initialize();
    }

    private void Initialize()
    {
        this._amountInputField.text = this._defaultSendAmount.ToString();
        
        if (!this._appState.LoggedIn())
        {
            this._sendButton.UI.interactable = false;
            
            return;
        }

        this._contractAddressInputField.text = this._appState.GetCurrentBlockchainContractData().ERC20Decimal18;
        this._receiverAddressInputField.text = this._appState.GetLoginData().UserAddress;
        this._sendButton.UI.interactable = true;
    }

    private void Send()
    {
        this._sendERC20.RaiseEvent(new FTTransactionData
        {
            Amount = this._amountInputField.text,
            ReceiverAddress = this._receiverAddressInputField.text,
            ContractAddress = this._contractAddressInputField.text,
        });
    }
}