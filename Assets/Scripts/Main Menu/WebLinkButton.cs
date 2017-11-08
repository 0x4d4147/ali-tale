using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WebLinkButton : MonoBehaviour, IPointerClickHandler
{
    public string link;

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        Application.OpenURL(link);
    }
}
