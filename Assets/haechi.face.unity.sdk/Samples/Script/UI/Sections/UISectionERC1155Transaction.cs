using System;
using TMPro;
using UnityEngine;

public class UISectionERC1155Transaction : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField _contractAddressInputField;
    [SerializeField] private TMP_InputField _tokenIdInputField;
    [SerializeField] private TMP_InputField _quantityInputField;
    [SerializeField] private TMP_InputField _receiverAddressInputField;
    [SerializeField] private UIButton _sendButton;
    
    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;
    
    [Header("Broadcast to")] 
    [SerializeField] private NFTTransactionDataChannelSO _sendERC1155;

    private void OnEnable()
    {
        this.Initialize();
        this._sendButton.OnClickEvent += this.Send;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._sendButton.OnClickEvent -= this.Send;
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
    }
    
    private void Initialize()
    {
        if (!this._appState.LoggedIn())
        {
            this._sendButton.UI.interactable = false;
            
            return;
        }

        this._contractAddressInputField.text = this._appState.GetCurrentBlockchainContractData().ERC1155;
        this._receiverAddressInputField.text = this._appState.GetLoginData().UserAddress;
        this._sendButton.UI.interactable = true;
    }

    private void Send()
    {
        this._sendERC1155.RaiseEvent(new NFTTransactionData
        {
            ReceiverAddress = this._receiverAddressInputField.text,
            ContractAddress = this._contractAddressInputField.text,
            TokenId = this._tokenIdInputField.text,
            Quantity = this._quantityInputField.text,
        });
    }
}