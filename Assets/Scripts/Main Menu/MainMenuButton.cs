using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    public MainMenuPanelManager panelManager;

    GameObject checkmark;

    void Awake()
    {
        checkmark = transform.GetChild(0).gameObject;
        checkmark.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelManager.OnFocusEnteredButton(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        panelManager.OnFocusEnteredButton(this);
    }

    public void SetCheckmarkEnabled(bool b)
    {
        checkmark.SetActive(b);
    }
}
