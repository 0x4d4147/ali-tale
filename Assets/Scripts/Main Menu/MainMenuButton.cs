using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainMenuButton : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerExitHandler, IDeselectHandler
{
    public MainMenuPanelManager panelManager;

    public HoverableImageButton hoverable;

    public void OnPointerEnter(PointerEventData eventData)
    {
        panelManager.OnFocusEnteredButton(this);
    }

    public void OnSelect(BaseEventData eventData)
    {
        panelManager.OnFocusEnteredButton(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverable.ShowNormal();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        hoverable.ShowNormal();
    }
}
