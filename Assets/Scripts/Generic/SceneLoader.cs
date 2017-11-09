using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public interface ISceneLoadDependency
{
	bool IsReadyForLoad();
}

public class SceneLoader : MonoBehaviour
{
    public GameObject[] persistentObjects;

    public float minLoadingTime = 2f;

	[SerializeField]
	private string startingSceneName;

	[SerializeField]
	private bool doNotLoadStartScene;

	public static SceneLoader instance;

	public UnityEngine.Object[] startSceneLoadDependencies;

	public event Action BeforeSceneUnload;
	public event Action AfterSceneLoad;

	private bool isBusySwitching = false;

    void Awake()
    {
        instance = this;
    }

	private IEnumerator Start()
	{
		if (doNotLoadStartScene) yield break;
		yield return LoadSceneIter(startingSceneName, CastStartDependencyList(startSceneLoadDependencies));
	}

	ISceneLoadDependency[] CastStartDependencyList(UnityEngine.Object[] objects)
	{
		ISceneLoadDependency[] castedList = new ISceneLoadDependency[objects.Length];
		for (int i = 0; i < castedList.Length; ++i)
		{
			castedList[i] = (ISceneLoadDependency) objects[i];
		}
		return castedList;
	}

	private IEnumerator LoadSceneIter(string sceneName, ISceneLoadDependency[] loadDependencies)
	{
		SetPersistentSceneObjectsEnabled(true);

        // To see the loading screen instead of a weird flash,
        // force a minimum amount of loading time.
        // Yeah, it's a necessary evil.
        float startTime = Time.time;
        while (Time.time - startTime < minLoadingTime)
            yield return null;

		yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

		Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);

		// While at least one dependency is not satisfied.
		bool areAllDependenciesSatisfied = false;
		while (!areAllDependenciesSatisfied)
		{
			Debug.LogFormat("SceneLoader:: Waiting for dependencies.");
			// Assume true.
			areAllDependenciesSatisfied = true;
			for (int i = 0; i < loadDependencies.Length; ++i)
			{
				Debug.LogFormat("SceneLoader:: Depencency {0} is ready? {1}.", loadDependencies[i].ToString(), loadDependencies[i].IsReadyForLoad());
				// If one is not ready, invalidate and break.
				if (!loadDependencies[i].IsReadyForLoad())
				{
					areAllDependenciesSatisfied = false;
					break;
				}
			}
			yield return null;
		}

		SetPersistentSceneObjectsEnabled(false);

		if (AfterSceneLoad != null) AfterSceneLoad.Invoke();

		SceneManager.SetActiveScene(loadedScene);
	}

	private IEnumerator SwitchSceneIter(string nextSceneName, ISceneLoadDependency[] loadDependencies)
	{
		isBusySwitching = true;

		SetPersistentSceneObjectsEnabled(true);

		if (BeforeSceneUnload != null) BeforeSceneUnload.Invoke();

		// Unload current scene.
		Scene currentScene = SceneManager.GetActiveScene();
		yield return SceneManager.UnloadSceneAsync(currentScene.buildIndex);

		// Load next scene.
		yield return LoadSceneIter(nextSceneName, loadDependencies);

		isBusySwitching = false;
	}

	public void LoadSceneAsync(string sceneName)
	{
		LoadSceneAsync(sceneName, new ISceneLoadDependency[0]);
	}

	public void LoadSceneAsync(string sceneName, ISceneLoadDependency[] loadDependencies)
	{
		if (!isBusySwitching)
		{
			// We assume an initial scene will have been loaded in Start() and
			// call SwitchSceneIter to switch it out with the next scene to load.
			StartCoroutine(SwitchSceneIter(sceneName, loadDependencies));
		}
		else
		{
			Debug.LogError("SceneLoader:: Busy, cannot load scene: " + sceneName);
		}
	}

	public void SetPersistentSceneObjectsEnabled(bool b)
	{
        for (int i = 0; i < persistentObjects.Length; ++i)
        {
            persistentObjects[i].SetActive(b);
        }
	}
}