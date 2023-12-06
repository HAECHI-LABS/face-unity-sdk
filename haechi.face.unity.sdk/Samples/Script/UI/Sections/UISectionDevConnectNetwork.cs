using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISectionDevConnectNetwork : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState; 
    
    [Header("UI References")] 
    [SerializeField] private TMP_Dropdown _envDropdown;
    [SerializeField] private TMP_Dropdown _blockchainDropdown;
    [SerializeField] private TMP_InputField _apiKeyInputField;
    [SerializeField] private TMP_InputField _privateKeyInputField;
    [SerializeField] private TMP_InputField _multiStageIdInputField;
    [SerializeField] private UIButton _connectButton;
    
    [Header("Broadcast to")]
    [SerializeField] private StringEventChannelSO _changeProfile;
    [SerializeField] private StringEventChannelSO _changeBlockchain;
    [SerializeField] private StringEventChannelSO _changeApiKey;
    [SerializeField] private StringEventChannelSO _changePrivateKey;
    [SerializeField] private StringEventChannelSO _changeMultiStageId;
    [SerializeField] private VoidEventChannelSO _connect;
    [SerializeField] private VoidEventChannelSO _switchNetwork;
    
    [Header("Listening on")]
    [SerializeField] private VoidEventChannelSO _onLogoutEvent;

    private bool _connected = false;

    private void OnEnable()
    {
        this._onLogoutEvent.OnEventRaised += this.Disconnect;
    }

    private void OnDisable()
    {
        this._onLogoutEvent.OnEventRaised -= this.Disconnect;
    }

    private void Start()
    {
        this._envDropdown.onValueChanged.AddListener(this.OnEnvChanges);
        this._blockchainDropdown.onValueChanged.AddListener(this.OnBlockchainChanges);
        this._apiKeyInputField.onValueChanged.AddListener(this.OnApiKeyChanges);
        this._privateKeyInputField.onValueChanged.AddListener(this.OnPrivateKeyChanges);
        this._multiStageIdInputField.onValueChanged.AddListener(this.OnMultiStageIdChanges);
        this._connectButton.OnClickEvent += this.ConnectOrSwitchNetwork;
        
        this.Initialize();
    }
    
    private void Initialize()
    {
        this._appState.GetAllEnvironments().ForEach(
            e => this._envDropdown.options.Add(new TMP_Dropdown.OptionData(e)));
        this._envDropdown.captionText.text = this._envDropdown.options[0].text;
        
        this._appState.GetAllBlockchains().ForEach(
            b => this._blockchainDropdown.options.Add(new TMP_Dropdown.OptionData(b.ToString())));
        this._blockchainDropdown.captionText.text = this._blockchainDropdown.options[0].text;
        
        this._apiKeyInputField.text = this._appState.GetApiKey();
    }

    private void OnEnvChanges(int dropdownIndex)
    {
        this._changeProfile.RaiseEvent(this._envDropdown.options[dropdownIndex].text);
    }

    private void OnBlockchainChanges(int dropdownIndex)
    {
        this._changeBlockchain.RaiseEvent(this._blockchainDropdown.options[dropdownIndex].text);
    }

    private void OnApiKeyChanges(string value)
    {
        this._changeApiKey.RaiseEvent(value);
    }

    private void OnPrivateKeyChanges(string value)
    {
        this._changePrivateKey.RaiseEvent(value);
    }
    
    private void OnMultiStageIdChanges(string value)
    {
        this._changeMultiStageId.RaiseEvent(value);
    }
    
    private void ConnectOrSwitchNetwork()
    {
        if (!this._connected)
        {
            this._connect.RaiseEvent();
            this._connected = true;
            this._connectButton.SetText("Switch Network");
            return;
        }
        
        this._switchNetwork.RaiseEvent();
    }
    
    private void Disconnect()
    {
        this._connected = false;
        this._connectButton.SetText("Connect Network");
    }
}