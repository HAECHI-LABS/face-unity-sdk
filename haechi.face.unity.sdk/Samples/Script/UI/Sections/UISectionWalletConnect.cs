using UnityEngine;

public class UISectionWalletConnect : MonoBehaviour
{
    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("UI References")]
    [SerializeField] private UIButton _walletConnectButton;
    
    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private LoginDataChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;
    
    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _walletConnect;
    
    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._walletConnectButton.OnClickEvent += this.WalletConnect;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._walletConnectButton.OnClickEvent -= this.WalletConnect;
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
    }
    
    private void Initialize(LoginData loginData)
    {
        this.Initialize();
    }

    private void Initialize()
    {
        if (!this._appState.LoggedIn())
        {
            this._walletConnectButton.UI.interactable = false;
            return;
        }
        
        this._walletConnectButton.UI.interactable = true;
    }
    
    private void WalletConnect()
    {
        this._walletConnect.RaiseEvent();   
    }
}