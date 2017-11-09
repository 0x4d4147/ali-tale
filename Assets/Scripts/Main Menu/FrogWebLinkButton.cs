using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FrogWebLinkButton : MonoBehaviour, IPointerClickHandler
{
    public RectTransform[] frogPositions;
    RectTransform frogTransform;

    public string link;
    public int clickCount = 8;
    int currentClickCount = 0;

    int lastPosition = 0;

    void Awake()
    {
        frogTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        ++currentClickCount;
        if (currentClickCount == clickCount)
        {
            Application.OpenURL(link);
        }
        else
        {
            RepositionFrog();
        }
    }

    void RepositionFrog()
    {
        int newPosition;
        while ((newPosition = Random.Range(0, frogPositions.Length)) == lastPosition) { }
        frogTransform.localPosition = frogPositions[newPosition].localPosition;
        lastPosition = newPosition;
    }
}
