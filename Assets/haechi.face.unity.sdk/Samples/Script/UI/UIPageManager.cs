using System.Collections.Generic;
using UnityEngine;

public enum PageId
{
    None,
    Main,
    ConnectAndLogin,
    WalletHome,
    BoraPortal,
    FTPage,
    NFTPage,
    SignMessagePage,
    WalletConnect,
}

public class UIPageManager : MonoBehaviour
{
    [SerializeField] private List<UIPage> _pages;

    [Header("Listening on")] 
    [SerializeField] private PageEventChannelSO _onPageLoad;

    [Header("Broadcast to")] 
    [SerializeField] private VoidEventChannelSO _pageLoaded;

    private void OnEnable()
    {
        this._onPageLoad.OnEventRaised += this.LoadPage;
    }
    
    private void OnDisable()
    {
        this._onPageLoad.OnEventRaised -= this.LoadPage;
    }

    private void Start()
    {
        this.LoadPage(PageId.Main);
    }

    private void LoadPage(PageId pageId)
    {
        this._pages.ForEach(page =>
        {
            if (page.Id.Equals(pageId))
            {
                page.gameObject.SetActive(true);
                this._pageLoaded.RaiseEvent();
                return;
            }
            page.gameObject.SetActive(false);
        });    
    }
    
    
}