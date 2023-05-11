using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

public class UISectionBoraPortal : MonoBehaviour
{
    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("UI References")]
    [SerializeField] private UIButton _connectBoraButton;
    [SerializeField] private UIButton _isConnectedButton;
    
    [Header("Listening on")]
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private LoginDataChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onNetworkSwitched;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _connectBora;
    [SerializeField] private VoidEventChannelSO _checkIsConnectedBora;

    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._connectBoraButton.OnClickEvent += this.ConnectBora;
        this._isConnectedButton.OnClickEvent += this.CheckIsConnectedBora;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
        this._onNetworkSwitched.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._connectBoraButton.OnClickEvent -= this.ConnectBora;
        this._isConnectedButton.OnClickEvent -= this.CheckIsConnectedBora;
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
        this._onNetworkSwitched.OnEventRaised -= this.Initialize;
    }
    
    private void Initialize(LoginData loginData)
    {
        this.Initialize();
    }

    private void Initialize()
    {
        if (!this._appState.LoggedIn())
        {
            this._connectBoraButton.UI.interactable = false;
            this._isConnectedButton.UI.interactable = false;
            return;
        }

        if (!this._appState.GetBlockchain().Equals(Blockchain.BORA))
        {
            this._connectBoraButton.UI.interactable = false;
            this._isConnectedButton.UI.interactable = false;
            return;
        }
        
        this._connectBoraButton.UI.interactable = true;
        this._isConnectedButton.UI.interactable = true;
    }

    private void ConnectBora()
    {
        this._connectBora.RaiseEvent();
    }

    private void CheckIsConnectedBora()
    {
        this._checkIsConnectedBora.RaiseEvent();
    }
}