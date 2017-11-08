using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADLGameJamLink : MonoBehaviour
{
    public string link;

    public void OnClick()
    {
        Application.OpenURL(link);
    }
}
