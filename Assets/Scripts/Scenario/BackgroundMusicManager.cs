using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance;

    public SceneLoader sceneLoader;

    AudioSource audioSource;

    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        sceneLoader.AfterSceneLoad += SceneLoader_AfterSceneLoad;
        sceneLoader.BeforeSceneUnload += SceneLoader_BeforeSceneUnload;
    }

    void OnDisable()
    {
        sceneLoader.AfterSceneLoad -= SceneLoader_AfterSceneLoad;
        sceneLoader.BeforeSceneUnload -= SceneLoader_BeforeSceneUnload;
    }

    void SceneLoader_BeforeSceneUnload ()
    {
        audioSource.Stop();
    }

    void SceneLoader_AfterSceneLoad ()
    {
        audioSource.Stop();
    }

    public void StartPlaying(string clipName)
    {
        AudioClip clip = Resources.Load<AudioClip>("Music/" + clipName);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarningFormat("BackgroundMusicManager:: clip '{0}' not found.", clipName);
        }
    }
}
