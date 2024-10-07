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
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private LoginDataChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;

    private List<UIBlockchainCheckbox> _uiBlockchainCheckboxList = new List<UIBlockchainCheckbox>();
    private List<BlockchainNetwork> _selectedBlockchainNetwork = new List<BlockchainNetwork>();

    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._onLoginSuccessEvent.OnEventRaised += this.Initialize;
        this._onLogoutSuccessEvent.OnEventRaised += this.Initialize;
    }

    private void OnDisable()
    {
        this._openWalletHomeAllBlockchainButton.UI.onClick.RemoveAllListeners();
        this._openWalletHomeSelectedBlockchainButton.UI.onClick.RemoveAllListeners();
        
        this._onPageLoaded.OnEventRaised -= this.Initialize;
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
            this.MakeButtonsDisable();
            return;
        }
        
        this.MakeButtonsInteractable();
        this._openWalletHomeAllBlockchainButton.UI.onClick.AddListener(this.OnOpenAllBlockchainWalletHome);
        this._openWalletHomeSelectedBlockchainButton.UI.onClick.AddListener(this.OnOpenSelectedBlockchainWalletHome);
    }

    private void Start()
    {
        this.ClearAllUIBlockchainCheckbox();
        
        this.InitializeUIBlockchainCheckbox(BlockchainNetworks.GetAllNetworks());
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
        this._selectedBlockchainNetwork.Clear();
        this._uiBlockchainCheckboxList.ForEach(uiBlockchainCheckbox =>
        {
            uiBlockchainCheckbox.OnValueChanged -= this.OnToggleUpdated;
            Destroy(uiBlockchainCheckbox.gameObject);
        });
        this._uiBlockchainCheckboxList.Clear();
    }

    private void InitializeUIBlockchainCheckbox(List<BlockchainNetwork> blockchainNetworks)
    {
        blockchainNetworks.ForEach(blockchainNetwork =>
        {
            UIBlockchainCheckbox uiBlockchainCheckbox = Instantiate(this._uiBlockchainCheckboxPrefab, this._checkboxContentTransform)
                .GetComponent<UIBlockchainCheckbox>();
            
            uiBlockchainCheckbox.Initialize(blockchainNetwork);
            
            uiBlockchainCheckbox.OnValueChanged += this.OnToggleUpdated;
            
            this._uiBlockchainCheckboxList.Add(uiBlockchainCheckbox);
        });
    }

    private void OnToggleUpdated(bool enabled, BlockchainNetwork blockchainNetwork)
    {
        if (enabled && !this._selectedBlockchainNetwork.Contains(blockchainNetwork))
        {
            this._selectedBlockchainNetwork.Add(blockchainNetwork);
            return;
        }
        
        // condition: enabled = false
        
        if (!this._selectedBlockchainNetwork.Contains(blockchainNetwork))
        {
            return;
        }
        
        this._selectedBlockchainNetwork.Remove(blockchainNetwork);
    }
    
    private void OnOpenAllBlockchainWalletHome()
    {
        FaceWalletManager.Instance.OpenWalletHome();
    }
    
    private void OnOpenSelectedBlockchainWalletHome()
    {
        FaceWalletManager.Instance.OpenWalletHome(this._selectedBlockchainNetwork);
    }
}