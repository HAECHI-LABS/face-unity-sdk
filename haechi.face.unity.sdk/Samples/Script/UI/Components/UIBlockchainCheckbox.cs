using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Toggle))]
public class UIBlockchainCheckbox : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private Text _label;
    
    private BlockchainNetwork _blockchainNetwork;
    private Toggle _toggle;

    public UnityAction<bool, BlockchainNetwork> OnValueChanged; 

    private void Awake()
    {
        this._toggle = this.GetComponent<Toggle>();
        this._toggle.onValueChanged.AddListener(this.ToggleChanged);
    }

    public void Initialize(BlockchainNetwork blockchainNetwork)
    {
        this._blockchainNetwork = blockchainNetwork;
        this._toggle.isOn = false;
        this._label.text = blockchainNetwork.ToString();
    }
    
    private void ToggleChanged(bool enabled)
    {
        this.OnValueChanged?.Invoke(enabled, this._blockchainNetwork);
    }
}