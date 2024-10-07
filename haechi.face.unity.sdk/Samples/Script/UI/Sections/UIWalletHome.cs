using System.Collections.Generic;
using face_unity.haechi.face.unity.sdk.Runtime.Utils;
using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.UI;

public class UIWalletHome : MonoBehaviour
{
    [SerializeField] private ReadOnlyAppState _appState;
    
    [Header("UI References")]
    [SerializeField] private Transform _checkboxContentTransform;
    [SerializeField] private GameObject _uiBlockchainCheckboxPrefab;
    
    [SerializeField] private Button _openWalletHomeAllBlockchainButton;
    [SerializeField] private Button _openWalletHomeSelectedBlockchainButton;

    [Header("Listening on")] 
    [SerializeField] private VoidEventChannelSO _onPageLoaded;
    [SerializeField] private VoidEventChannelSO _onLoginSuccessEvent;
    [SerializeField] private VoidEventChannelSO _onLogoutSuccessEvent;

    private List<UIBlockchainCheckbox> _uiBlockchainCheckboxList = new List<UIBlockchainCheckbox>();
    private List<BlockchainNetwork> _selectedBlockchainNetwork = new List<BlockchainNetwork>();

    private void Awake()
    {
        this._openWalletHomeAllBlockchainButton.onClick.AddListener(this.OnOpenAllBlockchainWalletHome);
        this._openWalletHomeSelectedBlockchainButton.onClick.AddListener(this.OnOpenSelectedBlockchainWalletHome);

        this._openWalletHomeAllBlockchainButton.interactable = false;
        this._openWalletHomeSelectedBlockchainButton.interactable = false;
    }

    private void OnEnable()
    {
        this._onPageLoaded.OnEventRaised += this.Initialize;
        this._onLoginSuccessEvent.OnEventRaised += this.MakeButtonsInteractable;
        this._onLogoutSuccessEvent.OnEventRaised += this.MakeButtonsDisable;
    }

    private void OnDisable()
    {
        this._onPageLoaded.OnEventRaised -= this.Initialize;
        this._onLoginSuccessEvent.OnEventRaised -= this.MakeButtonsInteractable;
        this._onLogoutSuccessEvent.OnEventRaised -= this.MakeButtonsDisable;
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
        
        this.InitializeUIBlockchainCheckbox(BlockchainNetworks.GetAllNetworks());
    }

    private void MakeButtonsInteractable()
    {
        this._openWalletHomeAllBlockchainButton.interactable = true;
        this._openWalletHomeSelectedBlockchainButton.interactable = true;
    }
    
    private void MakeButtonsDisable()
    {
        this._openWalletHomeAllBlockchainButton.interactable = false;
        this._openWalletHomeSelectedBlockchainButton.interactable = false;
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