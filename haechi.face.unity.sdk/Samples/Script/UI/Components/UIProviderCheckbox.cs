using haechi.face.unity.sdk.Runtime.Type;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Toggle))]
public class UIProviderCheckbox : MonoBehaviour
{
    [Header("UI References")] 
    [SerializeField] private Text _label;
    
    private LoginProviderType _provider;
    private Toggle _toggle;

    public UnityAction<bool, LoginProviderType> OnValueChanged; 

    private void Awake()
    {
        this._toggle = this.GetComponent<Toggle>();
        this._toggle.onValueChanged.AddListener(this.ToggleChanged);
    }

    public void Initialize(LoginProviderType provider)
    {
        this._provider = provider;
        this._toggle.isOn = false;
        this._label.text = _provider.ToString();
    }
    
    private void ToggleChanged(bool enabled)
    {
        this.OnValueChanged?.Invoke(enabled, this._provider);
    }
}