using UnityEngine;

public abstract class UIPage : MonoBehaviour
{
    [SerializeField] private PageId _pageId;

    public PageId Id => this._pageId;
}