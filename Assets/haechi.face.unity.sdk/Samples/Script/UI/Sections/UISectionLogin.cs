using System;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

public class UISectionLogin : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;

    [Header("UI References")] 
    [SerializeField] private UIButton _loginButton;

    [SerializeField] private UIButton _loginGoogleButton;
    
    [SerializeField] private UIButton _loginFacebookButton;
    
    [SerializeField] private UIButton _loginAppleButton;
    
    [SerializeField] private UIButton _getBalanceButton;
    
    [SerializeField] private UIButton _logoutButton;

    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onConnectedEvent;
    [SerializeField] private LoginDataChannelSO _onLoginedEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutEvent;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _login;
    [SerializeField] private StringEventChannelSO _socialLogin;
    [SerializeField] private VoidEventChannelSO _getBalance;
    [SerializeField] private VoidEventChannelSO _logout;

    private void Start()
    {
        this._loginButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = false;
    }

    private void OnEnable()
    {
        this._loginButton.OnClickEvent += this.Login;
        this._loginGoogleButton.OnClickEvent += this.GoogleLogin;
        this._loginFacebookButton.OnClickEvent += this.FacebookLogin;
        this._loginAppleButton.OnClickEvent += this.AppleLogin;
        this._getBalanceButton.OnClickEvent += this.GetBalance;
        this._logoutButton.OnClickEvent += this.Logout;
        
        this._onConnectedEvent.OnEventRaised += this.OnConnected;
        this._onLoginedEvent.OnEventRaised += this.OnLogined;
        this._onLogoutEvent.OnEventRaised += this.OnLogouted;
    }

    private void OnDisable()
    {
        this._loginButton.OnClickEvent -= this.Login;
        this._loginGoogleButton.OnClickEvent -= this.GoogleLogin;
        this._loginFacebookButton.OnClickEvent -= this.FacebookLogin;
        this._loginAppleButton.OnClickEvent -= this.AppleLogin;
        this._getBalanceButton.OnClickEvent -= this.GetBalance;
        this._logoutButton.OnClickEvent -= this.Logout;
        
        this._onConnectedEvent.OnEventRaised -= this.OnConnected;
        this._onLoginedEvent.OnEventRaised -= this.OnLogined;
        this._onLogoutEvent.OnEventRaised -= this.OnLogouted;
    }

    private void Login()
    {
        this._login.RaiseEvent();
    }

    private void GoogleLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Google);   
    }

    private void FacebookLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Facebook);
    }
    
    private void AppleLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Apple);
    }
    
    private void GetBalance()
    {
        this._getBalance.RaiseEvent();
    }
    
    private void Logout()
    {
        this._logout.RaiseEvent();
    }
    
    private void OnConnected()
    {
        this._loginButton.UI.interactable = true;
        this._loginGoogleButton.UI.interactable = true;
        this._loginFacebookButton.UI.interactable = true;
        this._loginAppleButton.UI.interactable = true;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = true;
    }

    private void OnLogined(LoginData loginData)
    {
        this._loginButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = true;
        this._logoutButton.UI.interactable = true;
    }
    
    private void OnLogouted()
    {
        this._loginButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = false;
    }
}