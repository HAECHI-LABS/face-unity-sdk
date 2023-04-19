using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Events/Page Event Channel")]
public class PageEventChannelSO : ScriptableObject
{
    public UnityAction<PageId> OnEventRaised;

    public void RaiseEvent(PageId pageId)
    {
        this.OnEventRaised?.Invoke(pageId);
    }
}