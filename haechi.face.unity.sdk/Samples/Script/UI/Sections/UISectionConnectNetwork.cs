using System;
using haechi.face.unity.sdk.Runtime.Type;
using NBitcoin;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISectionConnectNetwork : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState; 
    
    [Header("UI References")] 
    [SerializeField] private TMP_Dropdown _blockchainDropdown;
    [SerializeField] private UIButton _connectButton;
    
    [Header("Broadcast to")]
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
        this._blockchainDropdown.onValueChanged.AddListener(this.OnBlockchainChanges);
        this._connectButton.OnClickEvent += this.ConnectOrSwitchNetwork;
        
        this.Initialize();
    }
    
    private void Initialize()
    {
        BlockchainNetworks.GetAllNetworks().ForEach(n => this._blockchainDropdown.options.Add(new TMP_Dropdown.OptionData(n.ToString())));
        this._blockchainDropdown.captionText.text = this._blockchainDropdown.options[0].text;
    }

    private void OnBlockchainChanges(int dropdownIndex)
    {
        BlockchainNetwork blockchainNetwork = BlockchainNetworks.ValueOf(this._blockchainDropdown.options[dropdownIndex].text);
        this._appState.SetCurrentNetwork(blockchainNetwork);
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