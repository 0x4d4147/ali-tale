using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldCharacterLaunchScene : MonoBehaviour
{
	public GameObject player;
	public string sceneToLaunch;
	public string scenarioStartId = "1";
    public ExpressionIcon expressionIcon = ExpressionIcon.None;
    bool isActivated = false;

	OverworldCharacterController characterController;

	void Awake()
	{
		characterController = GetComponent<OverworldCharacterController>();
	}

	void OnTriggerEnter(Collider collider)
	{
		if (collider.gameObject == player) {
            isActivated = true;
            characterController.ShowIcon(expressionIcon);
		}
	}

	void OnTriggerExit(Collider collider)
	{
		if (collider.gameObject == player) {
            isActivated = false;
			characterController.ShowIcon(ExpressionIcon.None);
		}
	}

	void Update()
	{
        if ( isActivated &&
			Input.GetKeyDown(KeyCode.Return)
			|| Input.GetKeyDown(KeyCode.Space)
			|| Input.GetKeyDown(KeyCode.E)) {

			GameData.instance.startAtConversation = scenarioStartId;
			SceneLoader.instance.LoadSceneAsync(sceneToLaunch);
		}
	}
}
