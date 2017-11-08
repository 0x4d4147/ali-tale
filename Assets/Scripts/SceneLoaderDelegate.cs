using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderDelegate : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneLoader.instance.LoadSceneAsync(sceneName);
    }
}
