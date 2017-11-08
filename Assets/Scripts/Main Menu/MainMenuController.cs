using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject howToPlay;
    public GameObject credits;

    public MainMenuButton mainMenuFirstSelected;
    public GameObject howToPlayFirstSelected;
    public GameObject creditsFirstSelected;

    public EventSystem eventSystem;

    public MainMenuPanelManager mainMenuPanelManager;

    void Start()
    {
        OpenMainMenu();
    }

    public void OpenMainMenu()
    {
        mainMenu.SetActive(true);
        howToPlay.SetActive(false);
        credits.SetActive(false);

        var focusedBtn = mainMenuPanelManager.GetCurrentlyFocusedButton();
        if (focusedBtn != null)
        {
            mainMenuPanelManager.OnFocusEnteredButton(focusedBtn);
        }
        else
        {
            mainMenuPanelManager.OnFocusEnteredButton(mainMenuFirstSelected);
        }
    }

    public void OpenHowToPlay()
    {
        mainMenu.SetActive(false);
        howToPlay.SetActive(true);
        credits.SetActive(false);
        eventSystem.SetSelectedGameObject(howToPlayFirstSelected);
    }

    public void OpenCredits()
    {
        mainMenu.SetActive(false);
        howToPlay.SetActive(false);
        credits.SetActive(true);
        eventSystem.SetSelectedGameObject(creditsFirstSelected);
    }
}
