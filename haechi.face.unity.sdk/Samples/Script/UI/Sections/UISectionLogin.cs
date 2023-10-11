using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;

public class UISectionLogin : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private UIButton _loginButton;
    
    [SerializeField] private UIButton _loginWithSelectedProvidersButton;

    [SerializeField] private UIButton _loginGoogleButton;
    
    [SerializeField] private UIButton _loginFacebookButton;
    
    [SerializeField] private UIButton _loginAppleButton;

    [SerializeField] private UIButton _loginWithIdTokenButton;
    
    [SerializeField] private UIButton _getBalanceButton;
    
    [SerializeField] private UIButton _logoutButton;
    
    [SerializeField] private Transform _checkboxContentTransform;
    
    [SerializeField] private GameObject _uiProviderCheckboxPrefab;

    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onConnectedEvent;
    [SerializeField] private LoginDataChannelSO _onLoginedEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutEvent;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _login;
    [SerializeField] private ProvidersEventChannelSO _loginWithSelectedProviders;
    [SerializeField] private StringEventChannelSO _socialLogin;
    [SerializeField] private VoidEventChannelSO _getBalance;
    [SerializeField] private VoidEventChannelSO _logout;
    [SerializeField] private VoidEventChannelSO _loginWithIdToken;

    private List<UIProviderCheckbox> _uiProviderCheckboxList = new List<UIProviderCheckbox>();
    private List<LoginProviderType> _selectedProviders = new List<LoginProviderType>();
    
    private void Start()
    {
        this._loginButton.UI.interactable = false;
        this._loginWithSelectedProvidersButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._loginWithIdTokenButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = false;
        this.ClearAllUIProviderCheckbox();
        this.InitializeUIProviderCheckbox(EnumUtils.AllEnumAsList<LoginProviderType>());
    }

    private void OnEnable()
    {
        this._loginButton.OnClickEvent += this.Login;
        this._loginWithSelectedProvidersButton.OnClickEvent += this.LoginWithSelectedProviders;
        this._loginGoogleButton.OnClickEvent += this.GoogleLogin;
        this._loginFacebookButton.OnClickEvent += this.FacebookLogin;
        this._loginAppleButton.OnClickEvent += this.AppleLogin;
        this._loginWithIdTokenButton.OnClickEvent += this.LoginWithIdToken;
        this._getBalanceButton.OnClickEvent += this.GetBalance;
        this._logoutButton.OnClickEvent += this.Logout;
        
        this._onConnectedEvent.OnEventRaised += this.OnConnected;
        this._onLoginedEvent.OnEventRaised += this.OnLogined;
        this._onLogoutEvent.OnEventRaised += this.OnLogouted;
    }

    private void OnDisable()
    {
        this._loginButton.OnClickEvent -= this.Login;
        this._loginWithSelectedProvidersButton.OnClickEvent -= this.LoginWithSelectedProviders;
        this._loginGoogleButton.OnClickEvent -= this.GoogleLogin;
        this._loginFacebookButton.OnClickEvent -= this.FacebookLogin;
        this._loginAppleButton.OnClickEvent -= this.AppleLogin;
        this._loginWithIdTokenButton.OnClickEvent -= this.LoginWithIdToken;
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
    
    private void LoginWithSelectedProviders()
    {
        this._loginWithSelectedProviders.RaiseEvent(this._selectedProviders);
    }

    private void LoginWithIdToken()
    {
        this._loginWithIdToken.RaiseEvent();
    }
    
    private void ClearAllUIProviderCheckbox()
    {
        this._selectedProviders.Clear();
        this._uiProviderCheckboxList.ForEach(uiProviderCheckbox =>
        {
            uiProviderCheckbox.OnValueChanged -= this.OnToggleUpdated;
            Destroy(uiProviderCheckbox.gameObject);
        });
        this._uiProviderCheckboxList.Clear();
    }
    
    private void InitializeUIProviderCheckbox(List<LoginProviderType> providers)
    {
        providers.ForEach(provider =>
        {
            UIProviderCheckbox uiProviderCheckbox = Instantiate(this._uiProviderCheckboxPrefab, this._checkboxContentTransform)
                .GetComponent<UIProviderCheckbox>();
            
            uiProviderCheckbox.Initialize(provider);
            
            uiProviderCheckbox.OnValueChanged += this.OnToggleUpdated;
            
            this._uiProviderCheckboxList.Add(uiProviderCheckbox);
        });
    }

    private void OnToggleUpdated(bool enabled, LoginProviderType provider)
    {
        if (enabled && !this._selectedProviders.Contains(provider))
        {
            this._selectedProviders.Add(provider);
        }
        
        if (!enabled && this._selectedProviders.Contains(provider))
        {
            this._selectedProviders.Remove(provider);
        }
    
        this._selectedProviders.Sort();
    }

    private void GoogleLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Google.HostValue());   
    }

    private void FacebookLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Facebook.HostValue());
    }
    
    private void AppleLogin()
    {
        this._socialLogin.RaiseEvent(LoginProviderType.Apple.HostValue());
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
        this._loginWithSelectedProvidersButton.UI.interactable = true;
        this._loginGoogleButton.UI.interactable = true;
        this._loginFacebookButton.UI.interactable = true;
        this._loginAppleButton.UI.interactable = true;
        this._loginWithIdTokenButton.UI.interactable = true;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = true;
    }

    private void OnLogined(LoginData loginData)
    {
        this._loginButton.UI.interactable = false;
        this._loginWithSelectedProvidersButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._loginWithIdTokenButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = true;
        this._logoutButton.UI.interactable = true;
    }
    
    private void OnLogouted()
    {
        this._loginButton.UI.interactable = false;
        this._loginWithSelectedProvidersButton.UI.interactable = false;
        this._loginGoogleButton.UI.interactable = false;
        this._loginFacebookButton.UI.interactable = false;
        this._loginAppleButton.UI.interactable = false;
        this._loginWithIdTokenButton.UI.interactable = false;
        this._getBalanceButton.UI.interactable = false;
        this._logoutButton.UI.interactable = false;
    }
}