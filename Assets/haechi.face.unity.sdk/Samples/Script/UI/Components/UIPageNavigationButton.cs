using TMPro;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(UIButton))]
public class UIPageNavigationButton : MonoBehaviour
{
    private UIButton _uiButton;
    [SerializeField] private PageId _pageId;

    [SerializeField] private TextMeshProUGUI _labelText;

    [Header("Broadcasting to")] 
    [SerializeField] private PageEventChannelSO _navigateTo;

    // public UnityAction<PageId> OnNavigateEvent;

    private void OnEnable()
    {
        this._uiButton.OnClickEvent += this.Navigate;
    }
    
    private void OnDisable()
    {
        this._uiButton.OnClickEvent -= this.Navigate;
    }

    private void Awake()
    {
        this._uiButton = this.GetComponent<UIButton>();
    }

    public void Initialize(PageId pageId, string labelText)
    {
        this._pageId = pageId;
        this._labelText.text = labelText;
    }

    private void Navigate()
    {
        this._navigateTo.RaiseEvent(this._pageId);
    }
}