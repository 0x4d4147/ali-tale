using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuPanelManager : MonoBehaviour
{
    public MainMenuButton[] mainMenuButtons;
    public EventSystem eventSystem;
    MainMenuButton currentlyFocusedButton;

    public void OnFocusEnteredButton(MainMenuButton b)
    {
        if (!eventSystem.alreadySelecting)
        {
            eventSystem.SetSelectedGameObject(b.gameObject);
        }
        ClearCheckmarks();
        currentlyFocusedButton = b;
        currentlyFocusedButton.hoverable.ShowHover();
    }

    public void ClearCheckmarks()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].hoverable.ShowNormal();
        }
    }

    public MainMenuButton GetCurrentlyFocusedButton()
    {
        return currentlyFocusedButton;
    }
}
