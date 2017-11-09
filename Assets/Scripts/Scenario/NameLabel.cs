using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NameLabel : MonoBehaviour
{
    Image backgroundImage;
    TMP_Text nameText;

    void Awake()
    {
        backgroundImage = GetComponent<Image>();
        nameText = transform.GetChild(0).GetComponent<TMP_Text>();
    }

    public void SetName(string name)
    {
        nameText.text = name;
    }

    public void SetVisible(bool b)
    {
        backgroundImage.enabled = b;
        nameText.gameObject.SetActive(b);
    }

    public void Clear()
    {
        SetVisible(false);
        SetName("");
    }
}
