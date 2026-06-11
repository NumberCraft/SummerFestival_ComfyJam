using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class TabGroup : MonoBehaviour
{
    [SerializeField] private List<TabButton> tabButtons;

    [SerializeField] private bool isInteractionVisible;

    [SerializeField] private Sprite tabIdle;
    [SerializeField] private Sprite tabHover;
    [SerializeField] private Sprite tabActive;

    [SerializeField] private TabButton selectedTab;

    [SerializeField] private List<GameObject> objectsToSwap;
    [SerializeField] private ScrollRect scrollRect;

    private void OnEnable()
    {
        if (scrollRect == null)
        {
            scrollRect = GetComponentInChildren<ScrollRect>();
        }

        //tabButtons = GetComponentsInChildren<TabButton>().ToList();

        tabButtons[0].Select();
    }

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);

        button.Deselect();
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTabs();
        if(selectedTab == null || button != selectedTab)
        {
            if (isInteractionVisible)
                button.background.sprite = tabHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }
        selectedTab = button;

        selectedTab.Select();

        ResetTabs();
        if (isInteractionVisible)
            button.background.sprite = tabActive;
        int index = tabButtons.IndexOf(button);

        for(int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].SetActive(true);
                scrollRect.content = objectsToSwap[i].GetComponent<RectTransform>();
            }
            else
            {
                objectsToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            if (isInteractionVisible)
                button.background.sprite = tabIdle;
        }
    }
}
