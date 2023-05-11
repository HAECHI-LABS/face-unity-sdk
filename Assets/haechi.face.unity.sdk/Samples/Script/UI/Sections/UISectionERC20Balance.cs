using TMPro;
using UnityEngine;

public class UISectionERC20Balance : MonoBehaviour
{
    [SerializeField] private AppStateSO _appState;
    
    [Header("UI References")]
    [SerializeField] private TMP_InputField _balanceInputField;
    [SerializeField] private TMP_InputField _contractAddressInputField;
    [SerializeField] private UIButton _getBalanceButton;
    
    [Header("Listening on")] 
    [SerializeField] private LoginDataChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    
    [Header("Broadcast to")] 
    [SerializeField] private FTQueryDataChannelSO _getERC20Balance;

    [SerializeField] private StringEventChannelSO _onERC20BalanceUpdated;
    
    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._getBalanceButton.OnClickEvent += this.Send;
        this._onERC20BalanceUpdated.OnEventRaised += this.SetERC20Balance;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._getBalanceButton.OnClickEvent -= this.Send;
        this._onERC20BalanceUpdated.OnEventRaised -= this.SetERC20Balance;
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
            this._getBalanceButton.UI.interactable = false;
            
            return;
        }

        this._balanceInputField.text = this._appState.GetERC20Balance();
        this._contractAddressInputField.text = this._appState.GetCurrentBlockchainContractData().ERC20Decimal18;
        this._getBalanceButton.UI.interactable = true;
    }

    private void Send()
    {
        this._getERC20Balance.RaiseEvent(new FTQueryData
        {
            ContractAddress = this._contractAddressInputField.text,
        });
    }
    
    private void SetERC20Balance(string newBalance)
    {
        this._balanceInputField.text = this._appState.GetERC20Balance();
    }
}