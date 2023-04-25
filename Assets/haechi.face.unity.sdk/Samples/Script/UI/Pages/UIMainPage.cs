using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UIMainPageButtonData
{
    [Tooltip("버튼에 표시될 버튼 텍스트 라벨")]
    [SerializeField] private string _buttonLabel;
    
    [Tooltip("버튼 클릭했을 때 이동할 페이지 아이디")]
    [SerializeField] private PageId _pageId;

    public string ButtonLabel => this._buttonLabel;
    public PageId PageId => this._pageId;
}

public class UIMainPage : UIPage
{
    // [Tooltip("Main 페이지 버튼 정보")] 
    // [SerializeField] private List<UIMainPageButtonData> _buttonDataList;
    //
    // [SerializeField] private GameObject _navigationButtonPrefab;
    //
    // [SerializeField] private Transform _buttonsTransform;
    //
    // private List<UIPageNavigationButton> _uiPageNavigationButtons = new List<UIPageNavigationButton>();

    // private void Start()
    // {
    //     this.ClearAllButtons();
    //     this.InitializeButtons();
    // }
    //
    // private void ClearAllButtons()
    // {
    //     this._uiPageNavigationButtons.ForEach(uiButton =>
    //     {
    //         Destroy(uiButton.gameObject);
    //     });
    //
    //     this._uiPageNavigationButtons.Clear();
    // }
    //
    // private void InitializeButtons()
    // {
    //     this._buttonDataList.ForEach(buttonData =>
    //     {
    //         UIPageNavigationButton uiNavigationButton = Instantiate(this._navigationButtonPrefab, this._buttonsTransform)
    //             .GetComponent<UIPageNavigationButton>();
    //         
    //         uiNavigationButton.Initialize(buttonData.PageId, buttonData.ButtonLabel);
    //
    //         this._uiPageNavigationButtons.Add(uiNavigationButton);
    //     });
    // }
}