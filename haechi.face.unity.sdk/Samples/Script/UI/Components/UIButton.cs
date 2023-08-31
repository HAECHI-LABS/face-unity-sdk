using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(Button))]
public class UIButton : MonoBehaviour
{
    [HideInInspector] public Button UI;

    [SerializeField] private TextMeshProUGUI _text;

    public UnityAction OnClickEvent;

    protected void Awake()
    {
        this.UI = this.GetComponent<Button>();
        this.UI.onClick.AddListener(this.Click);
    }

    public void Click()
    {
        this.OnClickEvent?.Invoke();
    }

    public void SetText(string value)
    {
        this._text.text = value;
    }
}