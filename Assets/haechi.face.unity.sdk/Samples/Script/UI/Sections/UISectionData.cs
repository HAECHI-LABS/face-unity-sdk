using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UISectionData : MonoBehaviour
{
    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("UI References")] 
    [SerializeField] private UIDataField _userIdDataField;
    [SerializeField] private UIDataField _addressDataField;
    [SerializeField] private UIDataField _balanceDataField;
    [SerializeField] private UIDataField _resultDataField;

    [Header("Listening on")]
    [SerializeField] private VoidEventChannelSO _onLogout;

    [SerializeField] private LoginDataChannelSO _onLogined;
    [SerializeField] private StringEventChannelSO _onBalanceUpdated;
    [SerializeField] private StringEventChannelSO _onResultUpdated;

    private void OnEnable()
    {
        this.SetUserLoginData();
        this._onLogout.OnEventRaised += this.ClearData;
        this._onLogined.OnEventRaised += this.UpdateUserData;
        this._onBalanceUpdated.OnEventRaised += this.UpdateUserData;
        this._onResultUpdated.OnEventRaised += this.UpdateUserData;
    }

    private void OnDisable()
    {
        this._onLogout.OnEventRaised -= this.ClearData;
        this._onLogined.OnEventRaised -= this.UpdateUserData;
        this._onBalanceUpdated.OnEventRaised -= this.UpdateUserData;
        this._onResultUpdated.OnEventRaised -= this.UpdateUserData;
    }

    private void UpdateUserData(LoginData value)
    {
        this.SetUserLoginData();
    }

    private void UpdateUserData(string value)
    {
        this.SetUserLoginData();
    }

    private void SetUserLoginData()
    {
        if (this._appState.GetLoginData() == null)
        {
            this.ClearData();
            return;
        }
        this._userIdDataField.SetData($"{this._appState.GetLoginData().UserId}");
        this._addressDataField.SetData($"{this._appState.GetLoginData().UserAddress}");
        this._balanceDataField.SetData($"{this._appState.GetLoginData().Balance}");
        this._resultDataField.SetData($"{this._appState.GetLoginData().Result}");
    }

    private void ClearData()
    {
        this._userIdDataField.SetData($"");
        this._addressDataField.SetData($"");
        this._balanceDataField.SetData($"");
        this._resultDataField.SetData($"");
    }
}