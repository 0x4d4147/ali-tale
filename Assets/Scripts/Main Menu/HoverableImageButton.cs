using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverableImageButton : MonoBehaviour,
    IPointerEnterHandler,
    ISelectHandler,
    IPointerExitHandler,
    IDeselectHandler,
    IPointerClickHandler
{
    public Sprite normalState;
    public Sprite hoverState;

    public bool handleHover = true;

    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ShowNormal()
    {
        image.sprite = normalState;
    }

    public void ShowHover()
    {
        image.sprite = hoverState;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (handleHover) ShowHover();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (handleHover) ShowHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (handleHover) ShowNormal();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (handleHover) ShowNormal();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if (handleHover) ShowNormal();
    }
}
