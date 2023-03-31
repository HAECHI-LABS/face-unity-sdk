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
    
    private Blockchain _blockchain;
    private Toggle _toggle;

    public UnityAction<bool, Blockchain> OnValueChanged; 

    private void Awake()
    {
        this._toggle = this.GetComponent<Toggle>();
        this._toggle.onValueChanged.AddListener(this.ToggleChanged);
    }

    public void Initialize(Blockchain blockchain)
    {
        this._blockchain = blockchain;
        this._toggle.isOn = false;
        this._label.text = blockchain.ToString();
    }
    
    private void ToggleChanged(bool enabled)
    {
        this.OnValueChanged?.Invoke(enabled, this._blockchain);
    }
}