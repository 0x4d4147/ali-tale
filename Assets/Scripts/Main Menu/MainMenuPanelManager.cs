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
        b.SetCheckmarkEnabled(true);
        currentlyFocusedButton = b;
    }

    public void ClearCheckmarks()
    {
        for (int i = 0; i < mainMenuButtons.Length; i++)
        {
            mainMenuButtons[i].SetCheckmarkEnabled(false);
        }
    }

    public MainMenuButton GetCurrentlyFocusedButton()
    {
        return currentlyFocusedButton;
    }
}
