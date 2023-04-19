using System;
using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.UI;

public class UISectionWalletHome : MonoBehaviour
{
    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("UI References")]
    [SerializeField] private Transform _checkboxContentTransform;
    [SerializeField] private GameObject _uiBlockchainCheckboxPrefab;
    
    [SerializeField] private UIButton _openWalletHomeAllBlockchainButton;
    [SerializeField] private UIButton _openWalletHomeSelectedBlockchainButton;
    
    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;

    [Header("Broadcasting to")] 
    [SerializeField] private VoidEventChannelSO _openAllBlockchainWalletHome;
    [SerializeField] private BlockchainsEventChannelSO _openSelectedBlockchainWalletHome;
    
    private List<UIBlockchainCheckbox> _uiBlockchainCheckboxList = new List<UIBlockchainCheckbox>();
    private List<Blockchain> _selectedBlockchain = new List<Blockchain>();

    private void Awake()
    {
        this._openWalletHomeAllBlockchainButton.UI.onClick.AddListener(this.OnOpenAllBlockchainWalletHome);
        this._openWalletHomeSelectedBlockchainButton.UI.onClick.AddListener(this.OnOpenSelectedBlockchainWalletHome);

        this._openWalletHomeAllBlockchainButton.UI.interactable = false;
        this._openWalletHomeSelectedBlockchainButton.UI.interactable = false;
    }

    private void OnEnable()
    {
        this.Initialize();
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._onLoginSuccessEvent.OnEventRaised -= this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised -= this.Initialize;
    }

    private void Initialize()
    {
        if (!this._appState.LoggedIn())
        {
            this.MakeButtonsDisable();
            return;
        }
        
        this.MakeButtonsInteractable();
    }

    private void Start()
    {
        this.ClearAllUIBlockchainCheckbox();
        
        this.InitializeUIBlockchainCheckbox(EnumUtils.AllEnumAsList<Blockchain>());
    }
    
    private void MakeButtonsInteractable()
    {
        this._openWalletHomeAllBlockchainButton.UI.interactable = true;
        this._openWalletHomeSelectedBlockchainButton.UI.interactable = true;
    }
    
    private void MakeButtonsDisable()
    {
        this._openWalletHomeAllBlockchainButton.UI.interactable = false;
        this._openWalletHomeSelectedBlockchainButton.UI.interactable = false;
    }

    private void ClearAllUIBlockchainCheckbox()
    {
        this._selectedBlockchain.Clear();
        this._uiBlockchainCheckboxList.ForEach(uiBlockchainCheckbox =>
        {
            uiBlockchainCheckbox.OnValueChanged -= this.OnToggleUpdated;
            Destroy(uiBlockchainCheckbox.gameObject);
        });
        this._uiBlockchainCheckboxList.Clear();
    }

    private void InitializeUIBlockchainCheckbox(List<Blockchain> blockchains)
    {
        blockchains.ForEach(blockchain =>
        {
            UIBlockchainCheckbox uiBlockchainCheckbox = Instantiate(this._uiBlockchainCheckboxPrefab, this._checkboxContentTransform)
                .GetComponent<UIBlockchainCheckbox>();
            
            uiBlockchainCheckbox.Initialize(blockchain);
            
            uiBlockchainCheckbox.OnValueChanged += this.OnToggleUpdated;
            
            this._uiBlockchainCheckboxList.Add(uiBlockchainCheckbox);
        });
    }

    private void OnToggleUpdated(bool enabled, Blockchain blockchain)
    {
        if (enabled && !this._selectedBlockchain.Contains(blockchain))
        {
            this._selectedBlockchain.Add(blockchain);
            return;
        }
        
        // condition: enabled = false
        
        if (!this._selectedBlockchain.Contains(blockchain))
        {
            return;
        }
        
        this._selectedBlockchain.Remove(blockchain);
    }
    
    private void OnOpenAllBlockchainWalletHome()
    {
        this._openAllBlockchainWalletHome.RaiseEvent();
    }
    
    private void OnOpenSelectedBlockchainWalletHome()
    {
        this._openSelectedBlockchainWalletHome.RaiseEvent(this._selectedBlockchain);
    }
}